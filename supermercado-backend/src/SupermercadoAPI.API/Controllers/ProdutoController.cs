using Microsoft.AspNetCore.Mvc;
using MediatR;
using SupermercadoAPI.Application.Commands;
using SupermercadoAPI.Application.Queries;
using SupermercadoAPI.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace SupermercadoAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProdutoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProdutoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> ObterTodos()
        {
            var resultado = await _mediator.Send(new ObterTodosProdutosQuery());
            return Ok(resultado);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdutoDTO>> ObterPorId(Guid id)
        {
            var resultado = await _mediator.Send(new ObterProdutoPorIdQuery(id));
            if (resultado == null)
                return NotFound();

            return Ok(resultado);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> Adicionar([FromBody] AdicionarProdutoCommand command)
        {
            var resultado = await _mediator.Send(command);
            return Ok(resultado);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> Atualizar(Guid id, [FromBody] AtualizarProdutoCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var resultado = await _mediator.Send(command);
            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Excluir(Guid id)
        {
            var resultado = await _mediator.Send(new ExcluirProdutoCommand(id));
            return Ok(resultado);
        }
    }
}