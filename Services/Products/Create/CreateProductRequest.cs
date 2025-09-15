namespace App.Services.Products.Create
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; } 
    }
}
