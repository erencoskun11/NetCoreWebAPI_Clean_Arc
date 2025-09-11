using App.Services.Products.Create;
using App.Services.Products.Update;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace App.Services.Products
{
    public interface IProductService
    {
        Task<ServiceResult<List<ProductDto>>> GetTopPriceAsync(int count);
        Task<ServiceResult<List<ProductDto>>> GetAllListAsync();
        Task<ServiceResult<ProductDto?>> GetByIdAsync(int id);
        Task<ServiceResult<CreateProductResponse>> CreateProductAsync(CreateProductRequest request);
        Task<ServiceResult> UpdateProductAsync(int id, UpdateProductRequest request); // dikkat: (int, UpdateProductRequest)
        Task<ServiceResult> DeleteProductAsync(int id);
        Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize);
        Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request); // stok güncelleme ayrı metot
    }
}
