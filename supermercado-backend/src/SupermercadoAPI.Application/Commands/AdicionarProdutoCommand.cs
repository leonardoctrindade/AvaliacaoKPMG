using MediatR;

namespace SupermercadoAPI.Application.Commands
{
    public class AdicionarProdutoCommand : IRequest<Guid>
    {
        public string Nome { get; set; }
        public string Setor { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}
