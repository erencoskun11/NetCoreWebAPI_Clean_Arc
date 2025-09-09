using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Repositories.Products;
namespace App.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public Task<List<Product>> GetTopPriceProductAsync(int count);
    }
}
