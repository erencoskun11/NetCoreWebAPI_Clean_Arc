using App.Application.Features.Products.Dto;

namespace App.Application.Features.Categories.Dto
{
    public record CategoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public List<ProductDto> Products { get; init; } = new();
    }
}
