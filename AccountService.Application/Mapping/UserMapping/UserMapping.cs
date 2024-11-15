using AccountService.Domain.Dtos.UserDtos;
using AccountService.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Mapping.UserMapping
{
    public class UserMapping:Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id));

            CreateMap<User, UserInfoDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
                .ForCtorParam("Surname", o => o.MapFrom(s => s.Surname))
                .ForCtorParam("Patronymic", o => o.MapFrom(s => s.Patronymic))
                .ForCtorParam("PhoneNumber", o => o.MapFrom(s => s.PhoneNumber))
                .ForCtorParam("Role", o => o.MapFrom(s => s.Role.Title))
                .ReverseMap();
        }
    }
}
