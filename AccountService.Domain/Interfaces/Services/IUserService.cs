using AccountService.Domain.Dtos.UserDtos;
using AccountService.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<CollectionResult<UserInfoDto>> GetHRListAsync();
        Task<BaseResult<UserInfoDto>> GetMyProfileAsync(string id);
    }
}
