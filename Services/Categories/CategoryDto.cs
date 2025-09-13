using App.Services.Products;

namespace App.Services.Categories
{
    public record CategoryDto(int Id,string Name,List<ProductDto>Products);
   
}
