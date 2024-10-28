namespace SupermercadoAPI.Application.DTOs
{
    public class ProdutoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Setor { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}
