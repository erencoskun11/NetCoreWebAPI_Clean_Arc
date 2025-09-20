using App.API2.Filters;
using App.Application.Features.Categories;
using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Update;
using App.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace App.API2.Controllers
{

    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            return CreateActionResult(await _categoryService.GetAllListAsync());
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id) => CreateActionResult(await _categoryService.GetByIdAsync(id));

        [HttpGet("products")]
        public async Task<IActionResult> GetAllCategoriesWithProducts()
    => CreateActionResult(await _categoryService.GetAllCategoriesWithProductsAsync());

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetCategoryWithProducts(int id) 
            => CreateActionResult(await _categoryService.GetCategoryWithProductsAsync(id));


        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
        {
            return CreateActionResult(await _categoryService.CreateAsync(request));
        }
        
        [ServiceFilter(typeof(NotFoundFilter<Category, int>))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryRequest request) => CreateActionResult(await
            _categoryService.UpdateAsync(id, request));


        [ServiceFilter(typeof(NotFoundFilter<Category, int>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id) =>CreateActionResult(await _categoryService.DeleteAsync(id));




    } }