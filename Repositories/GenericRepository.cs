using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using App.Repositories.Products;

namespace App.Repositories
{
    public class GenericRepository<T,TId> : IGenericRepository<T,TId> where T : BaseEntity<TId> where TId : struct
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        //IQueryable means it does not fetch the data immediately
        public IQueryable<T> GetAll() => _dbSet.AsNoTracking();

        public Task<bool> AnyAsync(TId id) =>_dbSet.AnyAsync(x=>x.Id.Equals(id));

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).AsNoTracking();

        public ValueTask<T?> GetByIdAsync(int id) => _dbSet.FindAsync(id);

        public async ValueTask<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
