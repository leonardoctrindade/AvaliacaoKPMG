using SupermercadoAPI.API.Models;
using SupermercadoAPI.Domain.Entities;

namespace SupermercadoAPI.API.Repositories
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto?> GetByIdAsync(int id);
        Task<Produto> CreateAsync(Produto produto);
        Task<Produto> UpdateAsync(Produto produto);
        Task<bool> DeleteAsync(int id);
    }
}
