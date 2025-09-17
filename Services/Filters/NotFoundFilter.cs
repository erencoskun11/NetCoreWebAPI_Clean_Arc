using App.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using NHibernate.Mapping.ByCode;

namespace App.Services.Filters
{
    public class NotFoundFilter<T> : IAsyncActionFilter where T : class
    {
        public NotFoundFilter(IGenericRepository<T> genericRepository)
        {

        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var idValue = context.ActionArguments.Values.FirstOrDefault();

            if (idValue==null)
            {
                await next(); return;
            }
            var id = Convert.ToInt32(idValue);

            await next();
        }
    }
}
