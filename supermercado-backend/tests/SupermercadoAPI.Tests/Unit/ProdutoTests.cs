using Xunit;
using Moq;
using SupermercadoAPI.Domain.Entities;
using SupermercadoAPI.Domain.Interfaces;
using SupermercadoAPI.Application.Commands;
using SupermercadoAPI.Application.Handlers;
using SupermercadoAPI.Application.Queries;
using SupermercadoAPI.Application.DTOs;
using AutoMapper;
using FluentAssertions;
using SupermercadoAPI.Application.Validators;

namespace SupermercadoAPI.Tests.Unit
{
    public class ProdutoTests
    {
        public class DomainTests
        {
            [Fact]
            public void Produto_DeveCriarInstanciaValida()
            {
                // Arrange
                string nome = "Produto Teste";
                string setor = "Setor Teste";
                string descricao = "Descrição Teste";
                decimal preco = 10.99m;

                // Act
                var produto = new Produto(nome, setor, descricao, preco);

                // Assert
                produto.Should().NotBeNull();
                produto.Id.Should().NotBe(Guid.Empty);
                produto.Nome.Should().Be(nome);
                produto.Setor.Should().Be(setor);
                produto.Descricao.Should().Be(descricao);
                produto.Preco.Should().Be(preco);
            }

            [Fact]
            public void AtualizarDados_DeveAtualizarPropriedades()
            {
                // Arrange
                var produto = new Produto("Nome Original", "Setor Original", "Desc Original", 10.00m);
                string novoNome = "Novo Nome";
                string novoSetor = "Novo Setor";
                string novaDescricao = "Nova Descrição";
                decimal novoPreco = 20.00m;

                // Act
                produto.AtualizarDados(novoNome, novoSetor, novaDescricao, novoPreco);

                // Assert
                produto.Nome.Should().Be(novoNome);
                produto.Setor.Should().Be(novoSetor);
                produto.Descricao.Should().Be(novaDescricao);
                produto.Preco.Should().Be(novoPreco);
            }
        }

        public class CommandHandlerTests
        {
            private readonly Mock<IProdutoRepository> _repositoryMock;
            private readonly Mock<IMapper> _mapperMock;

            public CommandHandlerTests()
            {
                _repositoryMock = new Mock<IProdutoRepository>();
                _mapperMock = new Mock<IMapper>();
            }

            [Fact]
            public async Task AdicionarProdutoHandler_DeveAdicionarProdutoComSucesso()
            {
                // Arrange
                var command = new AdicionarProdutoCommand
                {
                    Nome = "Produto Teste",
                    Setor = "Setor Teste",
                    Descricao = "Descrição Teste",
                    Preco = 10.99m
                };

                var produto = new Produto(command.Nome, command.Setor, command.Descricao, command.Preco);
                var handler = new AdicionarProdutoCommandHandler(_repositoryMock.Object);

                _repositoryMock
                    .Setup(r => r.AdicionarAsync(It.IsAny<Produto>()))
                    .ReturnsAsync(produto.Id);

                // Act
                var resultado = await handler.Handle(command, CancellationToken.None);

                // Assert
                resultado.Should().NotBe(Guid.Empty);
                _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
            }

            [Fact]
            public async Task AtualizarProdutoHandler_DeveAtualizarProdutoComSucesso()
            {
                // Arrange
                var produtoId = Guid.NewGuid();
                var command = new AtualizarProdutoCommand
                {
                    Id = produtoId,
                    Nome = "Produto Atualizado",
                    Setor = "Setor Atualizado",
                    Descricao = "Descrição Atualizada",
                    Preco = 20.99m
                };

                var produtoExistente = new Produto("Nome Original", "Setor Original", "Desc Original", 10.00m);

                _repositoryMock
                    .Setup(r => r.ObterPorIdAsync(produtoId))
                    .ReturnsAsync(produtoExistente);

                var handler = new AtualizarProdutoCommandHandler(_repositoryMock.Object);

                // Act
                var resultado = await handler.Handle(command, CancellationToken.None);

                // Assert
                resultado.Should().BeTrue();
                _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Produto>()), Times.Once);
            }

            [Fact]
            public async Task ExcluirProdutoHandler_DeveExcluirProdutoComSucesso()
            {
                // Arrange
                var produtoId = Guid.NewGuid();
                var command = new ExcluirProdutoCommand(produtoId);
                var handler = new ExcluirProdutoCommandHandler(_repositoryMock.Object);

                // Act
                var resultado = await handler.Handle(command, CancellationToken.None);

                // Assert
                resultado.Should().BeTrue();
                _repositoryMock.Verify(r => r.ExcluirAsync(produtoId), Times.Once);
            }
        }

        public class QueryHandlerTests
        {
            private readonly Mock<IProdutoRepository> _repositoryMock;
            private readonly Mock<IMapper> _mapperMock;

            public QueryHandlerTests()
            {
                _repositoryMock = new Mock<IProdutoRepository>();
                _mapperMock = new Mock<IMapper>();
            }

            [Fact]
            public async Task ObterTodosProdutosHandler_DeveRetornarListaDeProdutos()
            {
                // Arrange
                var produtos = new List<Produto>
                {
                    new Produto("Produto 1", "Setor 1", "Desc 1", 10.00m),
                    new Produto("Produto 2", "Setor 2", "Desc 2", 20.00m)
                };

                var produtosDTO = new List<ProdutoDTO>
                {
                    new ProdutoDTO { Id = Guid.NewGuid(), Nome = "Produto 1", Setor = "Setor 1", Descricao = "Desc 1", Preco = 10.00m },
                    new ProdutoDTO { Id = Guid.NewGuid(), Nome = "Produto 2", Setor = "Setor 2", Descricao = "Desc 2", Preco = 20.00m }
                };

                _repositoryMock
                    .Setup(r => r.ObterTodosAsync())
                    .ReturnsAsync(produtos);

                _mapperMock
                    .Setup(m => m.Map<IEnumerable<ProdutoDTO>>(produtos))
                    .Returns(produtosDTO);

                var handler = new ObterTodosProdutosQueryHandler(_repositoryMock.Object);
                var query = new ObterTodosProdutosQuery();

                // Act
                var resultado = await handler.Handle(query, CancellationToken.None);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Should().HaveCount(2);
                _repositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
                _mapperMock.Verify(m => m.Map<IEnumerable<ProdutoDTO>>(produtos), Times.Once);
            }

            [Fact]
            public async Task ObterProdutoPorIdHandler_DeveRetornarProduto()
            {
                // Arrange
                var produtoId = Guid.NewGuid();
                var produto = new Produto("Produto Teste", "Setor Teste", "Desc Teste", 10.00m);
                var produtoDTO = new ProdutoDTO
                {
                    Id = produtoId,
                    Nome = "Produto Teste",
                    Setor = "Setor Teste",
                    Descricao = "Desc Teste",
                    Preco = 10.00m
                };

                _repositoryMock
                    .Setup(r => r.ObterPorIdAsync(produtoId))
                    .ReturnsAsync(produto);

                _mapperMock
                    .Setup(m => m.Map<ProdutoDTO>(produto))
                    .Returns(produtoDTO);

                var handler = new ObterProdutoPorIdQueryHandler(_repositoryMock.Object);
                var query = new ObterProdutoPorIdQuery(produtoId);

                // Act
                var resultado = await handler.Handle(query, CancellationToken.None);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Id.Should().Be(produtoId);
                _repositoryMock.Verify(r => r.ObterPorIdAsync(produtoId), Times.Once);
                _mapperMock.Verify(m => m.Map<ProdutoDTO>(produto), Times.Once);
            }
        }

        public class ValidatorTests
        {
            [Theory]
            [InlineData("")]
            [InlineData(null)]
            [InlineData(" ")]
            public void AdicionarProdutoCommand_NomeInvalido_DeveRetornarErro(string nomeInvalido)
            {
                // Arrange
                var command = new AdicionarProdutoCommand
                {
                    Nome = nomeInvalido,
                    Setor = "Setor Teste",
                    Descricao = "Descrição Teste",
                    Preco = 10.99m
                };

                var validator = new AdicionarProdutoValidator();

                // Act
                var resultado = validator.Validate(command);

                // Assert
                resultado.IsValid.Should().BeFalse();
                resultado.Errors.Should().Contain(e => e.PropertyName == nameof(command.Nome));
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            public void AdicionarProdutoCommand_PrecoInvalido_DeveRetornarErro(decimal precoInvalido)
            {
                // Arrange
                var command = new AdicionarProdutoCommand
                {
                    Nome = "Produto Teste",
                    Setor = "Setor Teste",
                    Descricao = "Descrição Teste",
                    Preco = precoInvalido
                };

                var validator = new AdicionarProdutoValidator();

                // Act
                var resultado = validator.Validate(command);

                // Assert
                resultado.IsValid.Should().BeFalse();
                resultado.Errors.Should().Contain(e => e.PropertyName == nameof(command.Preco));
            }
        }
    }
}