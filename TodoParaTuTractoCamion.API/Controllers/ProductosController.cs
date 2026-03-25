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

        [HttpGet("export-seed")]
        public async Task<IActionResult> ExportSeed([FromServices] TodoParaTuTractoCamion.Infrastructure.Persistence.TractoCamionDbContext context)
        {
            // Exportar datos locales a productos_backup.json
            var productos = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking(context.Productos).ToList();
            var dtos = productos.Select(p => new TodoParaTuTractoCamion.API.ProductoJsonDto(
                p.Id, p.Nombre, p.Precio.Value, p.Stock.Value, 
                p.Imagen1Url, p.Imagen2Url, p.Imagen3Url
            )).ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(dtos, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            
            var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "productos_backup.json");
            System.IO.File.WriteAllText(path, json);
            // También lo guardamos en la raíz del proyecto para asegurar
            System.IO.File.WriteAllText("productos_backup.json", json);

            return Ok(new { message = $"Exportados {dtos.Count} productos a JSON exitosamente.", path });
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
