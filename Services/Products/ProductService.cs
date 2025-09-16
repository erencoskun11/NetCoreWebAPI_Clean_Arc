using App.Repositories;
using App.Repositories.Products;
using App.Repositories.Categories;
using App.Services.Products.Create;
using App.Services.Products.Update;
using App.Services.Products.UpdateStock;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateProductRequest> _createProductRequestValidator;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateProductRequest> createProductRequestValidator,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _createProductRequestValidator = createProductRequestValidator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CreateProductResponse>> CreateProductAsync(CreateProductRequest request)
        {
            // Validator kontrolü
            var validationResult = await _createProductRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<CreateProductResponse>.Fail(errors, HttpStatusCode.BadRequest);
            }

            // Product ismi benzersiz mi?
            var anyProduct = await _productRepository.Where(x => x.Name == request.Name).AnyAsync();
            if (anyProduct)
                return ServiceResult<CreateProductResponse>.Fail("Product already exist", HttpStatusCode.BadRequest);

            // Category var mı kontrolü
            var categoryExists = await _categoryRepository.Where(c => c.Id == request.CategoryId).AnyAsync();
            if (!categoryExists)
                return ServiceResult<CreateProductResponse>.Fail($"Category not found with id: {request.CategoryId}", HttpStatusCode.BadRequest);

            var product = _mapper.Map<Product>(request);
            product.CategoryId = request.CategoryId;

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(
                new CreateProductResponse(product.Id),
                $"api/products/{product.Id}");
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            var products = await _productRepository.GetAll().ToListAsync();
            if (products == null || !products.Any())
                return ServiceResult<List<ProductDto>>.Fail("No products found", HttpStatusCode.NoContent);

            var dto = _mapper.Map<List<ProductDto>>(products);
            return ServiceResult<List<ProductDto>>.Success(dto, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            var products = await _productRepository.GetAll()
                                .Skip(skip)
                                .Take(pageSize)
                                .ToListAsync();

            if (products == null || !products.Any())
                return ServiceResult<List<ProductDto>>.Fail("No products found", HttpStatusCode.NoContent);

            var dto = _mapper.Map<List<ProductDto>>(products);
            return ServiceResult<List<ProductDto>>.Success(dto, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ServiceResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);

            var dto = _mapper.Map<ProductDto>(product);
            return ServiceResult<ProductDto?>.Success(dto);
        }

        public async Task<ServiceResult> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

            var anyProduct = await _productRepository
                .Where(x => x.Name == request.Name && x.Id != product.Id)
                .AnyAsync();
            if (anyProduct)
                return ServiceResult.Fail("Product name is already in database", HttpStatusCode.BadRequest);

            // Category kontrolü (opsiyonel)
            var categoryExists = await _categoryRepository.Where(c => c.Id == request.CategoryId).AnyAsync();
            if (!categoryExists)
                return ServiceResult.Fail($"Category not found with id: {request.CategoryId}", HttpStatusCode.BadRequest);

            product = _mapper.Map(request, product);
            product.CategoryId = request.CategoryId;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

            product.Stock = request.Quantity;
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceAsync(int count)
        {
            var products = await _productRepository.GetTopPriceProductAsync(count);
            if (products == null || !products.Any())
                return ServiceResult<List<ProductDto>>.Fail("Product not found.", HttpStatusCode.NotFound);

            var dto = _mapper.Map<List<ProductDto>>(products);
            return ServiceResult<List<ProductDto>>.Success(dto, HttpStatusCode.OK);
        }
    }
}

