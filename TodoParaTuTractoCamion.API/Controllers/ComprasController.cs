using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using TodoParaTuTractoCamion.Application.Purchases.Commands.ConfirmarCompra;

namespace TodoParaTuTractoCamion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ComprasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarCompra(ConfirmarCompraCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }
    }
}
