using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Domain.Entities;
using SupermercadoAPI.Domain.Interfaces;
using SupermercadoAPI.Infrastructure.Data;

namespace SupermercadoAPI.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly SupermercadoDbContext _context;

        public ProdutoRepository(SupermercadoDbContext context)
        {
            _context = context;
        }

        public async Task<Produto> ObterPorIdAsync(Guid id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task<IEnumerable<Produto>> ObterTodosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Guid> AdicionarAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto.Id;
        }

        public async Task AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Guid id)
        {
            var produto = await ObterPorIdAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
