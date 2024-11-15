using AccountService.Application.Resources;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using AccountService.Domain.Interfaces.Validations;
using AccountService.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Validations.FluentValidations.UserValidations
{
    public class UserValidator : IUserValidator
    {
        public BaseResult ValidateOnAlreadyExists(User model)
        {
            if (model == null)
            {
                return new BaseResult
                {
                    ErrorMessage = ErrorMessage.UserNotFounded,
                    ErrorCode = (int)ErrorCodes.UserNotFounded,
                };
            }
            return new BaseResult();
        }

        public BaseResult ValidateOnNull(User model)
        {
            if (model != null)
            {
                return new BaseResult
                {
                    ErrorMessage = ErrorMessage.UserNotFounded,
                    ErrorCode = (int)ErrorCodes.UserNotFounded,
                };
            }
            return new BaseResult();
        }
    }
}
