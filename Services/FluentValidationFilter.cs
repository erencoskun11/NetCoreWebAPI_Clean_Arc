using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Services
{
    public class FluentValidationFilter : IAsyncActionFilter
    {        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                  .SelectMany(e => e.Errors)
                  .Select(x => x.ErrorMessage).ToList();

                context.Result = new BadRequestObjectResult(new { Errors = errors });

                var resultModel = ServiceResult.Fail(errors);
                //context.Result = new BadRequestObjectResult(resultModel);
                return;
            }
            await next();
        }
    }
}
