using FluentValidation;

namespace App.Application.Features.Products.Update
{
    public class UpdateProductRequestValidatior : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidatior()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("product name is necessary")
                .Length(3, 10).WithMessage("product name mus be between 3 and 10.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("product price mus be greater than zero");

            RuleFor(x => x.Stock)
                .InclusiveBetween(1, 100).WithMessage("stock must be between 1 and 100");
        }

    }
}
