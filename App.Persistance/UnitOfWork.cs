using App.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace App.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            
            
                return await _context.SaveChangesAsync(cancellationToken);
         
        }
    }
}
