using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Areas.Admin.Controllers;

public class AccountController : Controller
{
    [Route("/admin/account/logout")]
    public IActionResult Logout()
    {
        return Redirect("~/account/logout");
    }
}