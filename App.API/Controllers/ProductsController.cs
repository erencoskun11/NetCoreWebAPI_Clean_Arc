using App.Repositories.Products;
using App.Services.Filters;
using App.Services.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using App.Services.Products.UpdateStock;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : CustomBaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreateActionResult<List<ProductDto>>(await _productService.GetAllListAsync());
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPagedAll(int pageNumber,int pageSize)
        {
            return CreateActionResult<List<ProductDto>>(await _productService.GetPagedAllListAsync(pageNumber,pageSize));
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            return CreateActionResult<ProductDto?>(await _productService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            return CreateActionResult<CreateProductResponse>(await _productService.CreateProductAsync(request));
        }
        [ServiceFilter(typeof(NotFoundFilter<Product, int>))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            return CreateActionResult(await _productService.UpdateProductAsync(id, request));
        }
        
        
        [HttpPatch("stock")]
        public async Task<IActionResult> UpdateStock(UpdateProductStockRequest request)
        {
            return CreateActionResult(await _productService.UpdateStockAsync(request));
        }

        [ServiceFilter(typeof(NotFoundFilter<Product,int>))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return CreateActionResult(await _productService.DeleteProductAsync(id));
        }

        [HttpGet("top/{count}")]
        public async Task<IActionResult> GetTopPriceProducts(int count)
        {
            return CreateActionResult<List<ProductDto>>(await _productService.GetTopPriceAsync(count));
        }

       }
}

