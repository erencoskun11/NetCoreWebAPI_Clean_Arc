using App.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace App.Services.Products.Create
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductRequestValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is necessary")
                .Length(3, 10).WithMessage("Product name must be between 3 and 10 characters.")
                .MustAsync(BeUniqueName).WithMessage("Product name already exists.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Product price must be greater than zero");

            RuleFor(x => x.Stock)
                .InclusiveBetween(1, 100).WithMessage("Product stock must be between 1 and 100");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        {
            // DB kontrolü asenkron
            return !await _productRepository.Where(x => x.Name == name).AnyAsync(cancellationToken);
        }
    }
}
//1. way 
//private bool MustUniqueProductName(string name)
//{
// return !_productrepository.Where(x => x.Name == name).Any();
//}