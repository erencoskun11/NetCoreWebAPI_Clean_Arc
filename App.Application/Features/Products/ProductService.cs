using App.Application.Contracts.Caching;
using App.Application.Contracts.Persistence;
using App.Application.Contracts.ServiceBus;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Dto;
using App.Application.Features.Products.Update;
using App.Application.Features.Products.UpdateStock;
using App.Domain.Entities;
using App.Domain.Events;
using AutoMapper;
using FluentValidation;
using System.Net;

namespace App.Application.Features.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateProductRequest> _createProductRequestValidator;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IServiceBus _busService;
        private const string ProductListCacheKey= "ProductListCacheKey";
        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateProductRequest> createProductRequestValidator,
            IMapper mapper,
            ICacheService cacheService,
            IServiceBus busService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _createProductRequestValidator = createProductRequestValidator;
            _mapper = mapper;
            _cacheService = cacheService;
            _busService = busService;
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
            var anyProduct = await _productRepository.AnyAsync(x => x.Name == request.Name);
            if (anyProduct)
                return ServiceResult<CreateProductResponse>.Fail("Product already exist", HttpStatusCode.BadRequest);

            // Category var mı kontrolü
            var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                return ServiceResult<CreateProductResponse>.Fail($"Category not found with id: {request.CategoryId}", HttpStatusCode.BadRequest);

            var product = _mapper.Map<Product>(request);
            product.CategoryId = request.CategoryId;

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            // cache temizleme
            await _cacheService.RemoveAsync(ProductListCacheKey);

            await _busService.PublishAsync(new ProductAddedEvent(product.Id, product.Name, product.Price));




            return ServiceResult<CreateProductResponse>.SuccessAsCreated(
                new CreateProductResponse(product.Id),
                $"api/products/{product.Id}");
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            // cache aside pattern
            // 1. cache kontrol
            var productListAsCached = await _cacheService.GetAsync<List<ProductDto>>(ProductListCacheKey);

            if (productListAsCached is not null)
                return ServiceResult<List<ProductDto>>.Success(productListAsCached);

            // 2. veritabanından çek
            var products = await _productRepository.GetAllAsync();

            if (products == null || !products.Any())
                return ServiceResult<List<ProductDto>>.Fail("No products found", HttpStatusCode.NoContent);

            // 3. DTO'ya dönüştür
            var dto = _mapper.Map<List<ProductDto>>(products);

            // 4. Cache’e ekle
            await _cacheService.AddAsync(ProductListCacheKey, dto, TimeSpan.FromMinutes(10));

            return ServiceResult<List<ProductDto>>.Success(dto, HttpStatusCode.OK);
        }


        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            var products = await _productRepository.GetAllPagedAsync(pageNumber,pageSize);

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
          
            var anyProduct = await _productRepository
                .AnyAsync(x => x.Name == request.Name && x.Id != id);
            if (anyProduct)
                return ServiceResult.Fail("Product name is already in database", HttpStatusCode.BadRequest);

            // Category kontrolü (opsiyonel)
            var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                return ServiceResult.Fail($"Category not found with id: {request.CategoryId}", HttpStatusCode.BadRequest);

            var product = _mapper.Map<Product>(request);
            product.Id = id;

            product = _mapper.Map(request, product);
            product.CategoryId = request.CategoryId;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
            // cache temizleme
            await _cacheService.RemoveAsync(ProductListCacheKey);
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
            // cache temizleme
            await _cacheService.RemoveAsync(ProductListCacheKey);
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
           
            _productRepository.Delete(product!);
            await _unitOfWork.SaveChangesAsync();
            // cache temizleme
            await _cacheService.RemoveAsync(ProductListCacheKey);
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

