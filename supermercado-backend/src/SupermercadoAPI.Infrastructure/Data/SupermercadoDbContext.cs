using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Domain.Entities;

namespace SupermercadoAPI.Infrastructure.Data
{
    public class SupermercadoDbContext : DbContext
    {
        public SupermercadoDbContext(DbContextOptions<SupermercadoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Setor).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descricao).HasMaxLength(500);
                entity.Property(e => e.Preco).HasPrecision(18, 2);
            });
        }
    }
}