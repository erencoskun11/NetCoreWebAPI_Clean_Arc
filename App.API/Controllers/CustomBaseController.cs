using App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(ServiceResult<T> result)
        {
            if (result.Status == HttpStatusCode.NoContent)
            {
                return new ObjectResult(null) { StatusCode = (int)result.Status };
            }

            return new ObjectResult(result) { StatusCode = (int)result.Status };
        }

        [NonAction]
        public IActionResult CreateActionResult<T>(ServiceResult result)
        {
            if (result.Status == HttpStatusCode.NoContent)
            {
                return new ObjectResult(null) { StatusCode = (int)result.Status };
            }

            return new ObjectResult(result) { StatusCode = (int)result.Status };
        }
    }
}
