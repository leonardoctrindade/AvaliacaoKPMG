using Microsoft.Extensions.DependencyInjection;
using SupermercadoAPI.Infrastructure.Data;
using SupermercadoAPI.Domain.Entities;
using SupermercadoAPI.Infrastructure.Repositories;
using SupermercadoAPI.Application.DTOs;
using MediatR;
using SupermercadoAPI.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SupermercadoAPI.API.Settings;
using SupermercadoAPI.API.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SupermercadoAPI.API.Models;

namespace SupermercadoAPI.Tests.Integration
{
    public class IntegrationTests : IDisposable
    {
        private readonly SupermercadoDbContext _context;
        private readonly API.Repositories.ProdutoRepository _repository;
        private readonly IMediator _mediator;
        private readonly ProdutoController _produtoController;
        private readonly AuthController _authController;

        public IntegrationTests()
        {
            var services = ConfigureServices();
            _context = services.GetRequiredService<SupermercadoDbContext>();
            _repository = services.GetRequiredService<API.Repositories.ProdutoRepository>();
            _mediator = services.GetRequiredService<IMediator>();
            _produtoController = services.GetRequiredService<ProdutoController>();
            _authController = services.GetRequiredService<AuthController>();

            // Garante que o banco está criado e limpo antes de cada teste
            _context.Database.EnsureCreated();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Configuração do DbContext em memória
            services.AddDbContext<SupermercadoDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()),
                ServiceLifetime.Singleton);

            // Configuração do Repository
            services.AddSingleton<API.Repositories.ProdutoRepository>();

            // Configuração do MediatR e Handlers
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AdicionarProdutoCommand).Assembly));

            // Configuração dos Controllers
            services.AddSingleton<ProdutoController>();

            // Configuração do JWT
            var jwtSettings = new JwtSettings
            {
                Secret = "CHAVE-SUPER-SECRETA-SUPERMERCADO-API",
                ExpiracaoHoras = 2,
                Emissor = "SupermercadoAPI",
                ValidoEm = "https://localhost"
            };
            services.AddSingleton<IOptions<JwtSettings>>(new OptionsWrapper<JwtSettings>(jwtSettings));
            services.AddSingleton<AuthController>();

            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task DeveExecutarFluxoCompletoDeProduto()
        {
            // 1. Adicionar um novo produto
            var comandoAdicionar = new AdicionarProdutoCommand
            {
                Nome = "Produto Teste",
                Setor = "Setor Teste",
                Descricao = "Descrição Teste",
                Preco = 99.99m
            };

            var resultadoAdicionar = await _produtoController.Adicionar(comandoAdicionar);
            var okResultAdicionar = Assert.IsType<OkObjectResult>(resultadoAdicionar.Result);
            var produtoId = Assert.IsType<Guid>(okResultAdicionar.Value);
            Assert.NotEqual(Guid.Empty, produtoId);

            // 2. Obter o produto por ID
            var resultadoObter = await _produtoController.ObterPorId(produtoId);
            var okResultObter = Assert.IsType<OkObjectResult>(resultadoObter.Result);
            var produtoObtido = Assert.IsType<ProdutoDTO>(okResultObter.Value);
            Assert.Equal(comandoAdicionar.Nome, produtoObtido.Nome);
            Assert.Equal(comandoAdicionar.Preco, produtoObtido.Preco);

            // 3. Atualizar o produto
            var comandoAtualizar = new AtualizarProdutoCommand
            {
                Id = produtoId,
                Nome = "Produto Atualizado",
                Setor = "Setor Atualizado",
                Descricao = "Descrição Atualizada",
                Preco = 149.99m
            };

            var resultadoAtualizar = await _produtoController.Atualizar(produtoId, comandoAtualizar);
            Assert.IsType<OkObjectResult>(resultadoAtualizar.Result);

            // 4. Verificar se a atualização foi aplicada
            var resultadoObterAtualizado = await _produtoController.ObterPorId(produtoId);
            var okResultObterAtualizado = Assert.IsType<OkObjectResult>(resultadoObterAtualizado.Result);
            var produtoAtualizado = Assert.IsType<ProdutoDTO>(okResultObterAtualizado.Value);
            Assert.Equal(comandoAtualizar.Nome, produtoAtualizado.Nome);
            Assert.Equal(comandoAtualizar.Preco, produtoAtualizado.Preco);

            // 5. Excluir o produto
            var resultadoExcluir = await _produtoController.Excluir(produtoId);
            Assert.IsType<OkObjectResult>(resultadoExcluir.Result);

            // 6. Verificar se o produto foi realmente excluído
            var resultadoObterExcluido = await _produtoController.ObterPorId(produtoId);
            Assert.IsType<NotFoundResult>(resultadoObterExcluido.Result);
        }

        [Fact]
        public async Task DeveRetornarTodosProdutos()
        {
            // Arrange
            var produtos = new List<Produto>
            {
                new Produto("Produto 1", "Setor 1", "Desc 1", 10.00m),
                new Produto("Produto 2", "Setor 2", "Desc 2", 20.00m),
                new Produto("Produto 3", "Setor 3", "Desc 3", 30.00m)
            };

            await _context.Produtos.AddRangeAsync(produtos);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _produtoController.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var produtosRetornados = Assert.IsAssignableFrom<IEnumerable<ProdutoDTO>>(okResult.Value);
            Assert.Equal(3, produtosRetornados.Count());
        }

        [Fact]
        public void DeveExecutarFluxoDeAutenticacao()
        {
            // 1. Tentar login com credenciais inválidas
            var loginInvalido = new API.Models.LoginModel
            {
                Email = "invalido@email.com",
                Senha = "senhaerrada"
            };

            var resultadoInvalido = _authController.Login(loginInvalido);
            Assert.IsType<UnauthorizedResult>(resultadoInvalido.Result);

            // 2. Login com credenciais válidas
            var loginValido = new API.Models.LoginModel
            {
                Email = "admin@admin.com",
                Senha = "admin"
            };

            var resultadoValido = _authController.Login(loginValido);
            var okResult = Assert.IsType<OkObjectResult>(resultadoValido.Result);
            var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);

            // Verificar o token e as claims
            Assert.NotNull(loginResponse.AccessToken);
            Assert.True(loginResponse.ExpiresIn > 0);
            Assert.Equal(loginValido.Email, loginResponse.UsuarioToken.Email);
            Assert.Contains(loginResponse.UsuarioToken.Claims,
                c => c == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            Assert.Contains(loginResponse.UsuarioToken.Claims,
                c => c == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        }

        [Fact]
        public async Task DeveLidarComErrosDeValidacao()
        {
            // Tentar adicionar produto com dados inválidos
            var comandoInvalido = new AdicionarProdutoCommand
            {
                Nome = "", // Nome vazio
                Setor = "", // Setor vazio
                Descricao = "Descrição",
                Preco = -10 // Preço negativo
            };

            // Se você tiver validação implementada, isso deve retornar um BadRequest
            var resultado = await _produtoController.Adicionar(comandoInvalido);
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task DeveLidarComConcorrencia()
        {
            // 1. Adicionar um produto
            var produto = new Produto("Produto Teste", "Setor Teste", "Descrição", 10.00m);
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();

            // 2. Simular duas atualizações concorrentes
            var comando1 = new AtualizarProdutoCommand
            {
                Id = produto.Id,
                Nome = "Atualização 1",
                Setor = produto.Setor,
                Descricao = produto.Descricao,
                Preco = produto.Preco
            };

            var comando2 = new AtualizarProdutoCommand
            {
                Id = produto.Id,
                Nome = "Atualização 2",
                Setor = produto.Setor,
                Descricao = produto.Descricao,
                Preco = produto.Preco
            };

            // Executar as atualizações
            await _produtoController.Atualizar(produto.Id, comando1);
            var resultado2 = await _produtoController.Atualizar(produto.Id, comando2);

            // Verificar o resultado final
            var produtoFinal = await _produtoController.ObterPorId(produto.Id);
            var produtoAtualizado = Assert.IsType<ProdutoDTO>(
                Assert.IsType<OkObjectResult>(produtoFinal.Result).Value);

            // A última atualização deve prevalecer
            Assert.Equal("Atualização 2", produtoAtualizado.Nome);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}