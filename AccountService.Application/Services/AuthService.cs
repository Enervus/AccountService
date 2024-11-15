using AccountService.Application.Resources;
using AccountService.Domain.Dtos.UserDtos;
using AccountService.Domain.Dtos.UserTokenDtos;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using AccountService.Domain.Interfaces.Repositories;
using AccountService.Domain.Interfaces.Services;
using AccountService.Domain.Interfaces.Validations;
using AccountService.Domain.Results;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Services
{
    public class AuthService:IAurhService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserToken> _tokenRepository;
        private readonly IUserValidator _userValidator;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public AuthService(IBaseRepository<User> userRepository, IBaseRepository<UserToken> tokenRepository, IUserValidator userValidator, ITokenService tokenService, IMapper mapper, ILogger logger)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _userValidator = userValidator;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<UserDto>> RegisterAsync(CreateUserDto dto)
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
            var result = _userValidator.ValidateOnAlreadyExists(user);
            if (!result.IsSuccess)
            {
                return new BaseResult<UserDto>
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode,
                };
            }

            user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Surname = dto.Surname,
                Patronymic = dto.Patronymic,
                PhoneNumber = dto.PhoneNumber,
                Password = HashPassword(dto.Password),
                RoleId = (int)Roles.HR,
                CreatedBy = dto.CreatedBy,
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new BaseResult<UserDto>
            {
                Data = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<BaseResult<TokenDto>> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
            var result = _userValidator.ValidateOnNull(user);
            if (!result.IsSuccess)
            {
                return new BaseResult<TokenDto>
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode,
                };
            }

            if (!IsVarifyPassword(dto.Password, user.Password))
            {
                _logger.Warning(ErrorMessage.InvalidPassword);
                return new BaseResult<TokenDto>
                {
                    ErrorMessage = ErrorMessage.InvalidPassword,
                    ErrorCode= (int)ErrorCodes.InvalidPassword
                };
            }

            var userToken = await _tokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

            var role = user.Role;
            var claims = new List<Claim> 
            { 
                new Claim("Id",user.Id),
                new Claim(ClaimTypes.Role, role.Title)
            };

            var refreshToken = _tokenService.GenerateRefreshToken();
            var accessToken = _tokenService.GenerateAccessToken(claims);

            if (userToken == null)
            {
                userToken = new UserToken
                {
                    Id = Guid.NewGuid().ToString(),
                    RefreshToken = refreshToken,
                    RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7),
                    UserId = user.Id
                };

                await _tokenRepository.CreateAsync(userToken);
                await _tokenRepository.SaveChangesAsync();
            }
            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);

                _tokenRepository.Update(userToken);
                await _tokenRepository.SaveChangesAsync();
            }

            return new BaseResult<TokenDto>
            {
                Data = new TokenDto(accessToken,refreshToken),
            };
        }

        public async Task<BaseResult<bool>> IsAuthorized(TokenDto dto)
        {
            string accessToken = dto.AccessToken;
            string refreshToken = dto.RefreshToken;

            var claimsPrincipal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null || user.UserToken.RefreshToken != refreshToken || user.UserToken.RefreshTokenExpireTime <= DateTime.UtcNow)
            {
                _logger.Warning(ErrorMessage.InvalidUserRequest);
                return new BaseResult<bool>
                {
                    ErrorMessage = ErrorMessage.InvalidUserRequest,
                    ErrorCode = (int)ErrorCodes.InvalidUserRequest,
                };
            }

            return new BaseResult<bool>
            {
                Data = true,
            };

        }

        private string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        private bool IsVarifyPassword(string password, string passwordHash)
        {
            return passwordHash == HashPassword(password);
        }
    }
}
