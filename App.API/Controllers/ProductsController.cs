using App.Services.Products;
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
            // Serviste manuel validasyon yapılıyor (ValidateAsync) — controller doğrudan servisi çağırır
            return CreateActionResult<CreateProductResponse>(await _productService.CreateProductAsync(request));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            return CreateActionResult<object>(await _productService.UpdateProductAsync(id, request));
        }

        [HttpPatch("stock")]
        public async Task<IActionResult> UpdateStock(UpdateProductStockRequest request)
        {
            return CreateActionResult<object>(await _productService.UpdateStockAsync(request));
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return CreateActionResult<object>(await _productService.DeleteProductAsync(id));
        }

        [HttpGet("top/{count}")]
        public async Task<IActionResult> GetTopPriceProducts(int count)
        {
            return CreateActionResult<List<ProductDto>>(await _productService.GetTopPriceAsync(count));
        }

       }
}

