using System;

namespace TodoParaTuTractoCamion.Application.DTOs
{
    public class ProductoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Imagen1Url { get; set; }
        public string Imagen2Url { get; set; }
        public string Imagen3Url { get; set; }
    }

    public class CompraDto
    {
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
