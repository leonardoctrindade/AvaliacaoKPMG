namespace SupermercadoAPI.API.Models
{
    public class UsuarioToken
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}
