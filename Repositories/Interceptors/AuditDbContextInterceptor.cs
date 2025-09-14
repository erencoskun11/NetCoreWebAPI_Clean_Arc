using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace App.Repositories.Interceptors
{
    public class AuditDbContextInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {


            foreach (var entityEntry in eventData.Context!.ChangeTracker.Entries().ToList())
            {

                switch(entityEntry.State)
                {
                    case EntityState.Added:

                    if(entityEntry.Entity is IAuditEntity auditEntity)
                        {
                            auditEntity.Created=DateTime.Now;
                            eventData.Context.Entry(auditEntity).Property(x => x.Updated).IsModified = false;
                        }



                        break;

                    case EntityState.Modified:

                        if (entityEntry.Entity is IAuditEntity auditUpdateEntity)
                        {
                            auditEntity.Created = DateTime.Now;
                            eventData.Context.Entry(auditEntity).Property(x => x.Updated).IsModified = false;
                        }


                        break;
                }








            }












          return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
