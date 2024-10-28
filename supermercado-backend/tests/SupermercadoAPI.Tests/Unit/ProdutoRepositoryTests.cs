using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Infrastructure.Data;
using SupermercadoAPI.Infrastructure.Repositories;
using SupermercadoAPI.Domain.Entities;
using SupermercadoAPI.Application.DTOs;
using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using SupermercadoAPI.Application.Commands;

namespace SupermercadoAPI.Tests.Unit
{
    public class ProdutoRepositoryTests
    {
        private SupermercadoDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SupermercadoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SupermercadoDbContext(options);
        }

        [Fact]
        public async Task AdicionarAsync_DeveAdicionarProdutoComSucesso()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ProdutoRepository(context);
            var produto = new Produto("Produto Teste", "Setor Teste", "Descrição Teste", 10.99m);

            // Act
            var id = await repository.AdicionarAsync(produto);

            // Assert
            var produtoSalvo = await context.Produtos.FindAsync(id);
            produtoSalvo.Should().NotBeNull();
            produtoSalvo.Nome.Should().Be(produto.Nome);
            produtoSalvo.Setor.Should().Be(produto.Setor);
            produtoSalvo.Descricao.Should().Be(produto.Descricao);
            produtoSalvo.Preco.Should().Be(produto.Preco);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarProdutoCorreto()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ProdutoRepository(context);
            var produto = new Produto("Produto Teste", "Setor Teste", "Descrição Teste", 10.99m);
            await context.Produtos.AddAsync(produto);
            await context.SaveChangesAsync();

            // Act
            var produtoObtido = await repository.ObterPorIdAsync(produto.Id);

            // Assert
            produtoObtido.Should().NotBeNull();
            produtoObtido.Id.Should().Be(produto.Id);
            produtoObtido.Nome.Should().Be(produto.Nome);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodosProdutos()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ProdutoRepository(context);

            var produtos = new[]
            {
                new Produto("Produto 1", "Setor 1", "Desc 1", 10.99m),
                new Produto("Produto 2", "Setor 2", "Desc 2", 20.99m),
                new Produto("Produto 3", "Setor 3", "Desc 3", 30.99m)
            };

            await context.Produtos.AddRangeAsync(produtos);
            await context.SaveChangesAsync();

            // Act
            var produtosObtidos = await repository.ObterTodosAsync();

            // Assert
            produtosObtidos.Should().HaveCount(3);
            produtosObtidos.Select(p => p.Nome).Should().Contain(produtos.Select(p => p.Nome));
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarProdutoComSucesso()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ProdutoRepository(context);
            var produto = new Produto("Produto Original", "Setor Original", "Desc Original", 10.99m);
            await context.Produtos.AddAsync(produto);
            await context.SaveChangesAsync();

            // Act
            produto.AtualizarDados("Produto Atualizado", "Setor Atualizado", "Desc Atualizada", 20.99m);
            await repository.AtualizarAsync(produto);

            // Assert
            var produtoAtualizado = await context.Produtos.FindAsync(produto.Id);
            produtoAtualizado.Should().NotBeNull();
            produtoAtualizado.Nome.Should().Be("Produto Atualizado");
            produtoAtualizado.Setor.Should().Be("Setor Atualizado");
            produtoAtualizado.Descricao.Should().Be("Desc Atualizada");
            produtoAtualizado.Preco.Should().Be(20.99m);
        }

        [Fact]
        public async Task ExcluirAsync_DeveExcluirProdutoComSucesso()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ProdutoRepository(context);
            var produto = new Produto("Produto Teste", "Setor Teste", "Desc Teste", 10.99m);
            await context.Produtos.AddAsync(produto);
            await context.SaveChangesAsync();

            // Act
            await repository.ExcluirAsync(produto.Id);

            // Assert
            var produtoExcluido = await context.Produtos.FindAsync(produto.Id);
            produtoExcluido.Should().BeNull();
        }
    }

    public class MappingTests
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configuration;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Produto, ProdutoDTO>();
                cfg.CreateMap<AdicionarProdutoCommand, Produto>();
                cfg.CreateMap<AtualizarProdutoCommand, Produto>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Fact]
        public void Configuration_DeveSerValida()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_ProdutoParaDTO_DeveMapearCorretamente()
        {
            // Arrange
            var produto = new Produto("Produto Teste", "Setor Teste", "Desc Teste", 10.99m);

            // Act
            var dto = _mapper.Map<ProdutoDTO>(produto);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(produto.Id);
            dto.Nome.Should().Be(produto.Nome);
            dto.Setor.Should().Be(produto.Setor);
            dto.Descricao.Should().Be(produto.Descricao);
            dto.Preco.Should().Be(produto.Preco);
        }

        [Fact]
        public void Map_CommandParaProduto_DeveMapearCorretamente()
        {
            // Arrange
            var command = new AdicionarProdutoCommand
            {
                Nome = "Produto Teste",
                Setor = "Setor Teste",
                Descricao = "Desc Teste",
                Preco = 10.99m
            };

            // Act
            var produto = _mapper.Map<Produto>(command);

            // Assert
            produto.Should().NotBeNull();
            produto.Nome.Should().Be(command.Nome);
            produto.Setor.Should().Be(command.Setor);
            produto.Descricao.Should().Be(command.Descricao);
            produto.Preco.Should().Be(command.Preco);
        }
    }
}