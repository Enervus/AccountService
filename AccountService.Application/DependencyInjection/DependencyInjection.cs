using AccountService.Application.Mapping.UserMapping;
using AccountService.Application.Services;
using AccountService.Application.Validations.FluentValidations.UserValidations;
using AccountService.Domain.Dtos.UserDtos;
using AccountService.Domain.Interfaces.Services;
using AccountService.Domain.Interfaces.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserMapping));
            services.InitServices();
        }

        public static void InitServices(this IServiceCollection services)
        {
            services.AddScoped<IAurhService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserValidator, UserValidator>();
            services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();
        }
    }
}
