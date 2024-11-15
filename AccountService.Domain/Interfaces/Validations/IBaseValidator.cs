using AccountService.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Interfaces.Validations
{
    public interface IBaseValidator<in T> where T : class
    {
        BaseResult ValidateOnNull(T model);
        BaseResult ValidateOnAlreadyExists(T model);
    }
}
