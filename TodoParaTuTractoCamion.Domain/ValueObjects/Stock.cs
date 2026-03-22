using TodoParaTuTractoCamion.Domain.Exceptions;

namespace TodoParaTuTractoCamion.Domain.ValueObjects
{
    public record Stock
    {
        public int Value { get; init; }

        private Stock() { } // EF Core

        public Stock(int value)
        {
            if (value < 0)
                throw new DomainException("El stock no puede ser negativo.");

            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
