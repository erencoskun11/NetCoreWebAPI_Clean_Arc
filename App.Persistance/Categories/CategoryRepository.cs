using App.Application.Contracts.Persistence;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Persistance.Categories
{
    public class CategoryRepository : GenericRepository<Category,int>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Category> GetCategoryWithProducts()
        {
            return _context.Categories.Include(x=>x.Products).AsNoTracking();
        }

        public Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return _context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
