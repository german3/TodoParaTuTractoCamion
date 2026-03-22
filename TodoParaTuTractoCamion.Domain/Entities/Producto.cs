using TodoParaTuTractoCamion.Domain.Exceptions;
using TodoParaTuTractoCamion.Domain.ValueObjects;

namespace TodoParaTuTractoCamion.Domain.Entities
{
    public class Producto
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public Precio Precio { get; private set; }
        public Stock Stock { get; private set; }
        public string? Imagen1Url { get; private set; }
        public string? Imagen2Url { get; private set; }
        public string? Imagen3Url { get; private set; }

        private Producto() { } // EF Core

        public Producto(Guid id, string nombre, Precio precio, Stock stock, string? img1, string? img2, string? img3)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new DomainException("El nombre del producto no puede estar vacío.");

            Id = id;
            Nombre = nombre;
            Precio = precio;
            Stock = stock;
            Imagen1Url = img1;
            Imagen2Url = img2;
            Imagen3Url = img3;
        }

        public void ValidarDisponibilidad(int cantidad)
        {
            if (Stock.Value < cantidad)
                throw new DomainException($"Stock insuficiente para el producto {Nombre}. Disponible: {Stock.Value}, Solicitado: {cantidad}");
        }

        public void ReducirStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new DomainException("La cantidad a reducir debe ser mayor a 0.");

            ValidarDisponibilidad(cantidad);
            Stock = new Stock(Stock.Value - cantidad);
        }

        public void AumentarStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new DomainException("La cantidad a aumentar debe ser mayor a 0.");

            Stock = new Stock(Stock.Value + cantidad);
        }
    }
}
