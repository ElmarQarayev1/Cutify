using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cutify.DATA
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext_>
    {
        public AppDbContext_ CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) 
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext_>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

            return new AppDbContext_(optionsBuilder.Options);
        }
    }
}

