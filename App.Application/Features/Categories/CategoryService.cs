using App.Application.Contracts.Caching;
using App.Application.Contracts.Persistence;
using App.Application.Contracts.ServiceBus;
using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Dto;
using App.Application.Features.Categories.Update;
using App.Domain.Entities;
using App.Domain.Events;
using AutoMapper;
using System.Net;

namespace App.Application.Features.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IServiceBus _serviceBus;
        private const string CategoryListCacheKey = "CategoryListCacheKey";
        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper,ICacheService cacheService,IServiceBus serviceBus)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _serviceBus = serviceBus;
        }

        public async Task<ServiceResult<CategoryWithProductsDto>> GetCategoryWithProductsAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryWithProductsAsync(categoryId);
            if (category == null)
                return ServiceResult<CategoryWithProductsDto>.Fail("The category was not found", HttpStatusCode.NotFound);

            var dto = _mapper.Map<CategoryWithProductsDto>(category);
            return ServiceResult<CategoryWithProductsDto>.Success(dto);
        }

        public async Task<ServiceResult<List<CategoryWithProductsDto>>> GetAllCategoriesWithProductsAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var dtos = _mapper.Map<List<CategoryWithProductsDto>>(categories);
            return ServiceResult<List<CategoryWithProductsDto>>.Success(dtos);
        }

        public async Task<ServiceResult<List<CategoryDto>>> GetAllListAsync()
        {
            var cache = await _cacheService.GetAsync<List<CategoryDto>>(CategoryListCacheKey);

            if(cache!= null) return ServiceResult<List<CategoryDto>>.Success(cache);
            
            var categories = await _categoryRepository.GetAllAsync();
            
            var dtos = _mapper.Map<List<CategoryDto>>(categories);
            
            await _cacheService.AddAsync(CategoryListCacheKey, dtos,TimeSpan.FromMinutes(10));
            return ServiceResult<List<CategoryDto>>.Success(dtos);
        }

        public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ServiceResult<CategoryDto>.Fail("The category was not found", HttpStatusCode.NotFound);

            var dto = _mapper.Map<CategoryDto>(category);
            return ServiceResult<CategoryDto>.Success(dto);
        }

        public async Task<ServiceResult<int>> CreateAsync(CreateCategoryRequest request)
        {
            var isDuplicate = await _categoryRepository.AnyAsync(x => x.Name == request.Name);
            if (isDuplicate)
                return ServiceResult<int>.Fail("The category name is already in database", HttpStatusCode.BadRequest);

            var newCategory = _mapper.Map<Category>(request);
            await _categoryRepository.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveAsync(CategoryListCacheKey);

            await _serviceBus.PublishAsync(new CategoryAddedEvent(newCategory.Id,newCategory.Name,newCategory.Created,newCategory.Updated));

            return ServiceResult<int>.Success(newCategory.Id);
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
        {
           
            var isDuplicate = await _categoryRepository
                .AnyAsync(x => x.Name == request.Name && x.Id != id);

            if (isDuplicate)
                return ServiceResult.Fail("Category name already exists", HttpStatusCode.BadRequest);

            var category = _mapper.Map<Category>(request);
            category.Id = id;
            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveAsync(CategoryListCacheKey);
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            _categoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveAsync(CategoryListCacheKey);
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
