using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.DAL.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Surname).IsRequired();
            builder.Property(x => x.Patronymic).IsRequired();
            builder.Property(x => x.PhoneNumber).IsRequired();
            builder.Property(x => x.Password).IsRequired().HasMaxLength(30);
            builder.Property(x => x.RoleId).IsRequired();

            builder.HasData(new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                Surname = "Admin",
                Patronymic = "Admin",
                PhoneNumber = "Admin",
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes("Admin")),
                RoleId = (int)Roles.Admin,
            });
        }
    }
}
