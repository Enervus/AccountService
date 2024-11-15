using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Dtos.UserDtos
{
    public record UserInfoDto(string Id, string Name, string Surname, string Patronymic, string PhoneNumber, string Role);
}
