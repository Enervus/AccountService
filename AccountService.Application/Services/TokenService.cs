using AccountService.Application.Resources;
using AccountService.Domain.Dtos.UserTokenDtos;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using AccountService.Domain.Interfaces.Repositories;
using AccountService.Domain.Interfaces.Services;
using AccountService.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Services
{
    public class TokenService: ITokenService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly ILogger _logger;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly double _lifetime;

        public TokenService(IBaseRepository<User> userRepository, ILogger logger, string jwtKey, string issuer, string audience, double lifetime)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtKey = jwtKey;
            _issuer = issuer;
            _audience = audience;
            _lifetime = lifetime;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var secutiryToken = new JwtSecurityToken(_issuer,_audience,claims,null,DateTime.UtcNow.AddMinutes(_lifetime), credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(secutiryToken);
            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumbers = new byte[32];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumbers);
            return Convert.ToBase64String(randomNumbers);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                ValidIssuer = _issuer,
                ValidAudience = _audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Warning(ErrorMessage.InvalidToken);
                throw new SecurityTokenException(ErrorMessage.InvalidToken);
            }

            return claimsPrincipal;
        }


        public async Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto)
        {
            string accessToken = dto.AccessToken;
            string refreshToken = dto.RefreshToken;

            var claimsPrincipal = GetPrincipalFromExpiredToken(accessToken);
            var userId = claimsPrincipal.Claims.FirstOrDefault(x=>x.Type == "Id")?.Value;
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
            if(user == null || user.UserToken.RefreshToken != refreshToken || user.UserToken.RefreshTokenExpireTime <= DateTime.UtcNow)
            {
                _logger.Warning(ErrorMessage.InvalidUserRequest);
                return new BaseResult<TokenDto>
                {
                    ErrorMessage = ErrorMessage.InvalidUserRequest,
                    ErrorCode = (int)ErrorCodes.InvalidUserRequest,
                };
            }

            var newAccessToken = GenerateAccessToken(claimsPrincipal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.UserToken.RefreshToken = newRefreshToken;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new BaseResult<TokenDto>
            {
                Data = new TokenDto(accessToken, refreshToken)
            };
        }
    }
}
