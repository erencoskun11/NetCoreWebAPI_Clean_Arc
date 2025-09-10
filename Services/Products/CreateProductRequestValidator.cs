using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Products
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is necessary")
                .Length(3, 10).WithMessage("Product name must be between 3 and 10 characters.");


            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Product name must be greater than zero");

            RuleFor(x => x.Stock)
                .InclusiveBetween(1,100).WithMessage("Product stock must be greater than zero ");
        
        
        }
    }
}
