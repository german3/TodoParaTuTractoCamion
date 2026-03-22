using FluentValidation;

namespace TodoParaTuTractoCamion.Application.Products.Commands.CreateProducto
{
    public class CreateProductoValidator : AbstractValidator<CreateProductoCommand>
    {
        public CreateProductoValidator()
        {
            RuleFor(v => v.Nombre).NotEmpty().MaximumLength(200);
            RuleFor(v => v.Precio).GreaterThan(0);
            RuleFor(v => v.Stock).GreaterThanOrEqualTo(0);
        }
    }
}
