using System.Collections.Generic;
using TodoParaTuTractoCamion.Domain.Exceptions;

namespace TodoParaTuTractoCamion.Domain.ValueObjects
{
    public record Precio
    {
        public decimal Value { get; init; }

        private Precio() { } // EF Core

        public Precio(decimal value)
        {
            if (value <= 0)
                throw new DomainException("El precio debe ser mayor a 0.");

            Value = value;
        }

        public override string ToString() => Value.ToString("C");
    }
}
