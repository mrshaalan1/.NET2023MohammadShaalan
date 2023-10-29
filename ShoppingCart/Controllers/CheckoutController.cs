using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModels;

namespace ShoppingCart.Controllers;

public class CheckoutController : Controller
{
    private readonly DataContext _context;

    public CheckoutController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        CartViewModel cartVM = new()
        {
            CartItems = cart,
            GrandTotal = cart.Sum(x => x.Quantity * x.Price)
        };

        OrderViewModel vm = new()
        {
            Order = new Order(),
            CartViewModel = cartVM
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(OrderViewModel orderViewModel)
    {
        List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();


        var newOrder = new Order()
        {
            FirstName = orderViewModel.Order.FirstName,
            LastName = orderViewModel.Order.LastName, 
            ShippingAddress = orderViewModel.Order.ShippingAddress,
            OrderNotes = orderViewModel.Order.OrderNotes,
            OrderDate = DateTime.Now,
            TotalAmount = cart.Sum(x => x.Quantity * x.Price),
            PaymentMethod = orderViewModel.Order.PaymentMethod,
            OrderItemsIds = cart.ToList(),
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };
        await _context.Orders.AddAsync(newOrder);
        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            TempData["Success"] = "The product has been added!";
            
            HttpContext.Session.Remove("Cart");

            
            return RedirectToAction("Index", "Home");
        }

        
        return BadRequest("error");
    }
}

