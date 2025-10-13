using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Repositories.Products;
using Microsoft.EntityFrameworkCore;

namespace App.Repositories
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
