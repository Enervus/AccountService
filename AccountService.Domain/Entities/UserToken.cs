using AccountService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Entities
{
    public class UserToken:IEntityId<string>
    {
        public string Id { get;set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
