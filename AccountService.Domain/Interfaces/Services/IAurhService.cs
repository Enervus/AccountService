using AccountService.Domain.Dtos.UserDtos;
using AccountService.Domain.Dtos.UserTokenDtos;
using AccountService.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Interfaces.Services
{
    public interface IAurhService
    {
        Task<BaseResult<UserDto>> RegisterAsync(CreateUserDto dto);
        Task<BaseResult<TokenDto>> LoginAsync(LoginUserDto dto);
        Task<BaseResult<bool>> IsAuthorized(TokenDto dto);
    }
}
