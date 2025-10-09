using App.Application.Contracts.Persistence;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Persistance.Products
{
    public class ProductRepository : GenericRepository<Product,int>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetTopPriceProductAsync(int count)
        {
            return await _context.Products
                                 .OrderByDescending(x => x.Price)
                                 .Take(count)
                                 .ToListAsync();
        }
    }
}
