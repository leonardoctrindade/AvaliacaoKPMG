using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermercadoAPI.Infrastructure.Data
{
    public class SupermercadoDbContextFactory : IDesignTimeDbContextFactory<SupermercadoDbContext>
    {
        public SupermercadoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SupermercadoDbContext>();

            optionsBuilder.UseSqlServer("DefaultConnection");

            return new SupermercadoDbContext(optionsBuilder.Options);
        }
    }
}
