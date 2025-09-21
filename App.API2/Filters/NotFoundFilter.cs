using App.Application;
using App.Application.Contracts.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.API2.Filters
{
    public class NotFoundFilter<T,TId> : Attribute,IAsyncActionFilter where T : class where TId : struct
    {
        private readonly IGenericRepository<T, TId> _repositoryRepository;
        public NotFoundFilter(IGenericRepository<T, TId> genericRepository)
        {
            _repositoryRepository = genericRepository;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var idValue = context.ActionArguments.Values.FirstOrDefault();

            var idKey = context.ActionArguments.Keys.First();


            if (idValue==null && idKey !="id")
            {
                await next(); return;
            }

            if (idValue is not TId id )
            {
                await next(); return;
            }
            var anyEntity = await _repositoryRepository.AnyAsync(id);


            if (!anyEntity)
            {
                var entityName = typeof(T).Name;

                //action method name
                var actionName = context.ActionDescriptor.RouteValues["action"];

                var result = ServiceResult.Fail($"Data not found({entityName})({actionName}).");
                context.Result = new NotFoundObjectResult(new { Message = "Entity nor found" });
                return;
            }



            await next();
        }
    }
}
