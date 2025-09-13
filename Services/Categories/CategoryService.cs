using App.Repositories;
using App.Repositories.Categories;
using App.Services.Categories.Create;
using App.Services.Categories.Update;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, UnitOfWork unitOfWork, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            var categories = await _categoryRepository.GetCategoryWithProducts().ToListAsync();
            var dtos = _mapper.Map<List<CategoryWithProductsDto>>(categories);
            return ServiceResult<List<CategoryWithProductsDto>>.Success(dtos);
        }

        public async Task<ServiceResult<List<CategoryDto>>> GetAllListAsync()
        {
            var categories = await _categoryRepository.GetAll().ToListAsync();
            var dtos = _mapper.Map<List<CategoryDto>>(categories);
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
            var isDuplicate = await _categoryRepository.Where(x => x.Name == request.Name).AnyAsync();
            if (isDuplicate)
                return ServiceResult<int>.Fail("The category name is already in database", HttpStatusCode.BadRequest);

            var newCategory = new Category { Name = request.Name };
            await _categoryRepository.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<int>.Success(newCategory.Id);
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ServiceResult.Fail("Category not found", HttpStatusCode.NotFound);

            var isDuplicate = await _categoryRepository
                .Where(x => x.Name == request.Name && x.Id != id)
                .AnyAsync();

            if (isDuplicate)
                return ServiceResult.Fail("Category name already exists", HttpStatusCode.BadRequest);

            _mapper.Map(request, category);
            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ServiceResult.Fail("The category was not found", HttpStatusCode.NotFound);

            _categoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
