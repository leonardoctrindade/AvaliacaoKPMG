using Xunit;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupermercadoAPI.API.Controllers;
using SupermercadoAPI.Application.Commands;
using SupermercadoAPI.Application.Queries;
using SupermercadoAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SupermercadoAPI.Tests.Controllers
{
    public class ProdutoControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProdutoController _controller;

        public ProdutoControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProdutoController(_mediatorMock.Object);
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarListaDeProdutos()
        {
            // Arrange
            var produtosEsperados = new List<ProdutoDTO>
            {
                new ProdutoDTO { Id = Guid.NewGuid(), Nome = "Produto 1", Setor = "Setor 1", Descricao = "Desc 1", Preco = 10.00m },
                new ProdutoDTO { Id = Guid.NewGuid(), Nome = "Produto 2", Setor = "Setor 2", Descricao = "Desc 2", Preco = 20.00m }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterTodosProdutosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(produtosEsperados);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ProdutoDTO>>(okResult.Value);
            Assert.Equal(produtosEsperados, returnValue);
        }

        [Fact]
        public async Task ObterPorId_QuandoProdutoExiste_DeveRetornarProduto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var produtoEsperado = new ProdutoDTO
            {
                Id = id,
                Nome = "Produto Teste",
                Setor = "Setor Teste",
                Descricao = "Descrição Teste",
                Preco = 15.00m
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterProdutoPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(produtoEsperado);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var returnValue = Assert.IsType<ProdutoDTO>(okResult.Value);
            Assert.Equal(produtoEsperado, returnValue);
        }

        [Fact]
        public async Task ObterPorId_QuandoProdutoNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterProdutoPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProdutoDTO)null);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            Assert.IsType<NotFoundResult>(resultado.Result);
        }

        [Fact]
        public async Task Adicionar_DeveRetornarIdDoProdutoAdicionado()
        {
            // Arrange
            var command = new AdicionarProdutoCommand
            {
                Nome = "Novo Produto",
                Setor = "Novo Setor",
                Descricao = "Nova Descrição",
                Preco = 25.00m
            };
            var idEsperado = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(idEsperado);

            // Act
            var resultado = await _controller.Adicionar(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var returnValue = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(idEsperado, returnValue);
        }

        [Fact]
        public async Task Atualizar_QuandoIdsCorrespondem_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new AtualizarProdutoCommand
            {
                Id = id,
                Nome = "Produto Atualizado",
                Setor = "Setor Atualizado",
                Descricao = "Descrição Atualizada",
                Preco = 30.00m
            };

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _controller.Atualizar(id, command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Atualizar_QuandoIdsDiferem_DeveRetornarBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var commandId = Guid.NewGuid();
            var command = new AtualizarProdutoCommand { Id = commandId };

            // Act
            var resultado = await _controller.Atualizar(id, command);

            // Assert
            Assert.IsType<BadRequestResult>(resultado.Result);
        }

        [Fact]
        public async Task Excluir_DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock
                .Setup(m => m.Send(It.Is<ExcluirProdutoCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _controller.Excluir(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.True((bool)okResult.Value);
        }
    }
}