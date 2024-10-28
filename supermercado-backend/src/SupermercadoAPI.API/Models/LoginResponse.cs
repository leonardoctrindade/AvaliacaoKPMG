namespace SupermercadoAPI.API.Models
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UsuarioToken UsuarioToken { get; set; }
    }
}
