using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using System;
using TodoParaTuTractoCamion.Application.Products.Queries.GetProductos;
using TodoParaTuTractoCamion.Application.Products.Queries.GetProductoById;
using TodoParaTuTractoCamion.Application.Products.Commands.CreateProducto;
using TodoParaTuTractoCamion.Application.Products.Commands.ReduceStock;

namespace TodoParaTuTractoCamion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mediator.Send(new GetProductosQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductoByIdQuery(id));
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductoCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("reducir-stock")]
        public async Task<IActionResult> ReduceStock(ReduceStockCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        // --- ENDPOINTS TEMPORALES PARA ADMINISTRACIÓN DE DATOS ---

        public record Dto(Guid id, string nombre, decimal precio, int stock, string imagen1Url, string imagen2Url, string imagen3Url);

        [HttpPost("seed-direct")]
        public async Task<IActionResult> SeedDirect([FromBody] List<Dto> payload, [FromServices] TodoParaTuTractoCamion.Infrastructure.Persistence.TractoCamionDbContext context)
        {
            if (payload == null || !payload.Any()) return BadRequest("No data");
            
            var productos = payload.Select(d => new TodoParaTuTractoCamion.Domain.Entities.Producto(
                d.id, d.nombre, 
                new TodoParaTuTractoCamion.Domain.ValueObjects.Precio(d.precio), 
                new TodoParaTuTractoCamion.Domain.ValueObjects.Stock(d.stock), 
                d.imagen1Url, d.imagen2Url, d.imagen3Url
            )).ToList();

            context.Productos.AddRange(productos);
            await context.SaveChangesAsync();
            return Ok($"Insertados {productos.Count} productos directamente.");
        }

        [HttpDelete("reset-seed")]
        public async Task<IActionResult> ResetSeed([FromServices] TodoParaTuTractoCamion.Infrastructure.Persistence.TractoCamionDbContext context)
        {
            // Borra todo para forzar resiembra en el próximo inicio
            context.Productos.RemoveRange(context.Productos);
            await context.SaveChangesAsync();
            return Ok("Base de datos borrada exitosamente. Reinicia la aplicación en Render para auto-sembrar los nuevos datos.");
        }
    }
}
