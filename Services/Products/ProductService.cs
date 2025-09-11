using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using App.Repositories.Products;
using App.Repositories;
using FluentValidation;

using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel.DataAnnotations;
using NHibernate.Transform;
using App.Services.Products.Create;
using App.Services.Products.Update;
using AutoMapper;
namespace App.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateProductRequest> _createProductRequestValidator;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IValidator<CreateProductRequest> createProductRequestValidator,IMapper mapper)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _createProductRequestValidator = createProductRequestValidator;
            _mapper = mapper;
        }


        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            // Validator'ı manuel olarak çalıştır
            var validationResult = await _createProductRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<CreateProductResponse>.Fail(errors, HttpStatusCode.BadRequest);
            }

            // (Opsiyonel) business validation burada da yapılabilir (ör. duplicate check)
            var anyProduct = await _productRepository.Where(x => x.Name == request.Name).AnyAsync();
            if (anyProduct)
            {
                return ServiceResult<CreateProductResponse>.Fail("Product already exist", HttpStatusCode.BadRequest);
            }

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock
            };

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(
                new CreateProductResponse(product.Id),
                $"api/products/{product.Id}");
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            var products = await _productRepository.GetAll().ToListAsync();

            if (products is null || !products.Any())
                return ServiceResult<List<ProductDto>>.Fail("No products found", HttpStatusCode.NoContent);

            /*var dto = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();
            */
            var dto =_mapper.Map<List<ProductDto>>(products);




            return ServiceResult<List<ProductDto>>.Success(dto, HttpStatusCode.OK);
        }
            
        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;

            var products = await _productRepository
                .GetAll()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            if (products is null || !products.Any())
                return ServiceResult<List<ProductDto>>.Fail("No products found", HttpStatusCode.NoContent);

            var dto = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();

            return ServiceResult<List<ProductDto>>.Success(dto, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                return ServiceResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);

            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };

            return ServiceResult<ProductDto?>.Success(dto, HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult<CreateProductResponse>> CreateProductAsync(CreateProductRequest request)
        {
            // Manuel olarak asenkron validasyon çalıştır
            var validationResult = await _createProductRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<CreateProductResponse>.Fail(errors, HttpStatusCode.BadRequest);
            }

            // Business kontrol (isteğe bağlı, validator'da zaten benzersizlik var ama double-check istersen)
            var anyProduct = await _productRepository.Where(x => x.Name == request.Name).AnyAsync();
            if (anyProduct)
            {
                return ServiceResult<CreateProductResponse>.Fail("Product already exist", HttpStatusCode.BadRequest);
            }

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock
            };

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(
                new CreateProductResponse(product.Id),
                $"api/products/{product.Id}");
        }


        public async Task<ServiceResult> UpdateProductAsync(int id ,UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);

            if(product is null)
            {
                return ServiceResult.Fail("Produc not found", HttpStatusCode.NotFound);
            }
            product.Stock = request.Quantity;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);

        }

        public async Task<ServiceResult> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceAsync(int count)
        {
            var products = await _productRepository.GetTopPriceProductAsync(count);

            if (products is null || !products.Any())
                return ServiceResult<List<ProductDto>>.Fail("Product not found.", HttpStatusCode.NotFound);

            var dto = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();

            return ServiceResult<List<ProductDto>>.Success(dto, HttpStatusCode.OK);
        }

    }
}
