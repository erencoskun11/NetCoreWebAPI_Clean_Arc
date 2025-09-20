using App.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace App.API2.ExceptionHandlers
{
    public class CriticalExceptionHandler() : IExceptionHandler
    {
        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is CriticalException)
            {
                Console.WriteLine("An SMS regarding the error has been sent");
            }
         return ValueTask.FromResult(false);
        }
    }
}
