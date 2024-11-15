using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.DAL
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly string secrets = "";
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AccountService"))
                .AddUserSecrets(secrets)
                .Build();

            var connectionsString = configuration.GetConnectionString("PostgreSQL");
            optionsBuilder.UseNpgsql(connectionsString, o=>o.MigrationsAssembly("Booking.DAL"));
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
