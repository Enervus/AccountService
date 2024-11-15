using AccountService.Application.Resources;
using AccountService.Domain.Dtos.UserDtos;
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
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Services
{
    internal class UserService : IUserService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IUserValidator _userValidator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public UserService(IBaseRepository<User> userRepository, IUserValidator userValidator, IMapper mapper, ILogger logger)
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CollectionResult<UserInfoDto>> GetHRListAsync()
        {
            UserInfoDto[] users;
            users = await _userRepository.GetAll().Where(x => x.RoleId == (int)Roles.HR).Select(x => new UserInfoDto(x.Id, x.Name, x.Surname, x.Patronymic, x.PhoneNumber, x.Role.Title)).ToArrayAsync();
            if (!users.Any())
            {
                _logger.Warning(ErrorMessage.UsersNotFounded, users.Length);
                return new CollectionResult<UserInfoDto>()
                {
                    ErrorMessage = ErrorMessage.UsersNotFounded,
                    ErrorCode = (int)ErrorCodes.UsersNotFounded
                };
            }

            return new CollectionResult<UserInfoDto>
            {
                Data = users,
                Count = users.Length,
            };
        }

        public async Task<BaseResult<UserInfoDto>> GetMyProfileAsync(string id)
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var result = _userValidator.ValidateOnNull(user);
            if (!result.IsSuccess)
            {
                _logger.Warning(result.ErrorMessage);
                return new BaseResult<UserInfoDto>
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode,
                };
            }

            return new BaseResult<UserInfoDto>
            {
                Data = _mapper.Map<UserInfoDto>(user),
            };
        }
    }
}
