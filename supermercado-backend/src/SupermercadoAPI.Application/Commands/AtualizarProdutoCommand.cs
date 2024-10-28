using MediatR;

namespace SupermercadoAPI.Application.Commands
{
    public class AtualizarProdutoCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Setor { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}
