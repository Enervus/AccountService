using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Dtos.UserDtos
{
    public record LoginUserDto(string PhoneNumber, string Password);
}
