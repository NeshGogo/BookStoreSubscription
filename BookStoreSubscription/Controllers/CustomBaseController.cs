using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreSubscription.Controllers
{

    public class CustomBaseController : ControllerBase
    {
        protected string GetUserId()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id");
            var userId = userClaim?.Value;
            return userId;
        }
    }
}
