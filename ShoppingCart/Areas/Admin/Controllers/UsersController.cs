using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure;

namespace ShoppingCart.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]

public class UsersController : Controller
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
        
    }

}