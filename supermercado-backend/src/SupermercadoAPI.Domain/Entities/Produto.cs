namespace SupermercadoAPI.Domain.Entities
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Setor { get; private set; }
        public string Descricao { get; private set; }
        public decimal Preco { get; private set; }

        public Produto(string nome, string setor, string descricao, decimal preco)
        {
            Id = Guid.NewGuid();
            AtualizarDados(nome, setor, descricao, preco);
        }

        public void AtualizarDados(string nome, string setor, string descricao, decimal preco)
        {
            Nome = nome;
            Setor = setor;
            Descricao = descricao;
            Preco = preco;
        }
    }
}
