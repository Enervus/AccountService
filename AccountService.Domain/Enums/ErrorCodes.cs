using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Enums
{
    public enum ErrorCodes
    {
        InvalidPassword = 1,
        UserNotFounded = 2,
        UsersNotFounded = 3,
        InvalidUserRequest = 4,

        InvalidToken = 11,

        InternalServerError = 500,
    }
}
