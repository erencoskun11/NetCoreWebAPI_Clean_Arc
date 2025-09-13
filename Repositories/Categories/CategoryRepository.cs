using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace App.Repositories.Categories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Category> GetCategoryWithProducts()
        {
            return _context.Categories.Include(x=>x.Products).AsQueryable();
        }

        public Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return _context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
