using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModels;

namespace ShoppingCart.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly DataContext _context;

    public HomeController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<DataPoint> dataPoints = new List<DataPoint>();

        var orders = await _context.Orders.ToListAsync();
        decimal sum = 0;
        dataPoints.Add(new DataPoint(0, 0));

        foreach (var order in orders)
        {
            sum += order.TotalAmount;
            dataPoints.Add(new DataPoint((int)(order.Id), sum));
        }

        ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints);


        var products = await _context.Products.ToListAsync();


        var uniqueProductIds = _context.CartItems
            .Select(ci => ci.ProductId)
            .Distinct()
            .ToList();

        decimal maxTotalSum = 0;
        int numberOfProductSold = 0;
        long productIdWithMaxTotalSum = 0;

        foreach (var productId in uniqueProductIds)
        {
            var cartItemsForProductId = _context.CartItems
                .Where(ci => ci.ProductId == productId)
                .ToList();

            decimal totalSum = cartItemsForProductId.Sum(ci => ci.Total);

            if (totalSum > maxTotalSum)
            {
                maxTotalSum = totalSum;
                productIdWithMaxTotalSum = productId;
                numberOfProductSold = cartItemsForProductId.Sum(ci => ci.Quantity);
            }
        }


        var ordersWithCartItems = await _context.Orders
            .Include(order => order.OrderItemsIds)
            .ToListAsync();
        var userWithMaxTotalSpending = ordersWithCartItems
            .GroupBy(order => order.userId)
            .Select(group => new
            {
                UserId = group.Key,
                TotalSpending = group.Sum(order => order.OrderItemsIds.Sum(cartItem => cartItem.Total))
            }).MaxBy(result => result.TotalSpending);

        var user = new AppUser();
        var winningProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == productIdWithMaxTotalSum);
        if (userWithMaxTotalSpending != null)
        {
            user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userWithMaxTotalSpending.UserId);
        }

        var vm = new AdminViewModel();

        if (userWithMaxTotalSpending == null)
        {
            vm = new AdminViewModel()
            {
                WinningProduct = winningProduct,
                NumberOfProductSold = numberOfProductSold,
                TotalRevenue = sum,
                LoyalCustomer = user,
                MaxSpent = 0
            };
        }
        else
        {
            vm = new AdminViewModel()
            {
                WinningProduct = winningProduct,
                NumberOfProductSold = numberOfProductSold,
                TotalRevenue = sum,
                LoyalCustomer = user,
                MaxSpent = userWithMaxTotalSpending.TotalSpending
            };
        }

        return View(vm);
    }
}