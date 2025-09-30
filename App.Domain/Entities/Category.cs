using App.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public class Category :BaseEntity<int>, IAuditEntity
    {
        public string Name { get; set; } = default!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
