using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SupermercadoAPI.API.Controllers;
using SupermercadoAPI.API.Models;
using SupermercadoAPI.API.Settings;
using System;

namespace SupermercadoAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly JwtSettings _jwtSettings;

        public AuthControllerTests()
        {
            _jwtSettings = new JwtSettings
            {
                Secret = "CHAVE-SUPER-SECRETA-SUPERMERCADO-API",
                ExpiracaoHoras = 2,
                Emissor = "SupermercadoAPI",
                ValidoEm = "https://localhost"
            };

            var options = new Mock<IOptions<JwtSettings>>();
            options.Setup(x => x.Value).Returns(_jwtSettings);

            _controller = new AuthController(options.Object);
        }

        [Fact]
        public void Login_ComCredenciaisValidas_DeveRetornarToken()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "admin@admin.com",
                Senha = "admin"
            };

            // Act
            var resultado = _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);

            Assert.NotNull(loginResponse.AccessToken);
            Assert.Equal(TimeSpan.FromHours(_jwtSettings.ExpiracaoHoras).TotalSeconds, loginResponse.ExpiresIn);
            Assert.Equal("admin@admin.com", loginResponse.UsuarioToken.Email);
            Assert.Contains(loginResponse.UsuarioToken.Claims, c => c == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            Assert.Contains(loginResponse.UsuarioToken.Claims, c => c == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        }

        [Theory]
        [InlineData("wrong@email.com", "admin")]
        [InlineData("admin@admin.com", "wrong")]
        [InlineData("wrong@email.com", "wrong")]
        public void Login_ComCredenciaisInvalidas_DeveRetornarUnauthorized(string email, string senha)
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = email,
                Senha = senha
            };

            // Act
            var resultado = _controller.Login(loginModel);

            // Assert
            Assert.IsType<UnauthorizedResult>(resultado.Result);
        }
    }
}