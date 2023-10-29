using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string categorySlug = "", int p = 1, bool orderByPrice = false,
            string search = "")
        {
            if (User.IsInRole("Admin"))
            {
                return Redirect("/admin/");
            }

            int pageSize = 3;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.CategorySlug = categorySlug;
            ViewBag.SearchTerm = search;
            ViewBag.OrderByPrice = orderByPrice;

            IQueryable<Product> productsQuery = _context.Products;

            if (!string.IsNullOrWhiteSpace(categorySlug))
            {
                Category category = await _context.Categories
                    .Where(c => c.Slug == categorySlug)
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    ViewBag.TotalPages = (int)Math.Ceiling((decimal)productsQuery.Count() / pageSize);

                    return RedirectToAction("Index");
                }

                productsQuery = productsQuery.Where(p => p.CategoryId == category.Id);
                ViewBag.TotalPages = (int)Math.Ceiling((decimal)productsQuery.Count() / pageSize);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Description.ToLower().Contains(search) ||
                    p.Category.Name.ToLower().Contains(search)
                );
            }

            int totalProducts;

            if (orderByPrice)
            {
                var productsList = await productsQuery.ToListAsync();
                productsList = productsList.OrderBy(p => p.Price).ToList();

                totalProducts = productsList.Count();

                ViewBag.TotalPages = (int)Math.Ceiling((decimal)totalProducts / pageSize);

                var products = productsList
                    .Skip((p - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return View(products);
            }
            else
            {
                totalProducts = await productsQuery.CountAsync();

                ViewBag.TotalPages = (int)Math.Ceiling((decimal)totalProducts / pageSize);

                var products = await productsQuery
                    .OrderByDescending(p => p.Id)
                    .Skip((p - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return View(products);
            }
        }

        [Route("/products/details")]
        public async Task<IActionResult> Details(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            return View(product);
        }

        [Route("/products/Rate")]
        public async Task<IActionResult> Rate(Product product)
        {
            var temp = await _context.Products.FirstOrDefaultAsync(x => x.Id == product.Id);

            var oldRating = temp.Rating;

            if (oldRating == 0)
            {
                temp.Rating = product.Rating;
            }
            else
            {
                var newRating = (oldRating + product.Rating) / 2;

                temp.Rating = newRating;
            }

            temp.RatingCount += 1;


            _context.Update(temp);
            var res = await _context.SaveChangesAsync() > 0;

            if (res)
            {
                TempData["Success"] = "Rating Submitted!";
                return RedirectToAction("Index", "Products");
            }

            return BadRequest("Bad");
        }

        [Route("/products/Review")]
        public async Task<IActionResult> Review(Product product)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var productId = product.Id;

            var body = product.body;

            await _context.Reviews.AddAsync(new Review()
            {
                UserId = userId,
                ProductId = productId,
                body = body
            });

            var res = await _context.SaveChangesAsync() > 0;

            if (res)
            {
                TempData["Success"] = "The Review has been submitted!";
            }

            return RedirectToAction("Index", "Products");
        }
        // [Route("/products/ReadReviews")]

        public async Task<IActionResult> ReadReviews(long id)
        {
            var reviews = await _context.Reviews
                .Include(r => r.AppUser).Where(r => r.ProductId == id)
                .ToListAsync();
            // var reviews = _context.Reviews.Where(r => r.ProductId == id).Include(x => x.AppUser.Id ).ToList();
            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == rev)
            return View(reviews);
        }

        // public async Task<IActionResult> Index(string categorySlug = "", int p = 1)
        // {
        //         int pageSize = 3;
        //         ViewBag.PageNumber = p;
        //         ViewBag.PageRange = pageSize;
        //         ViewBag.CategorySlug = categorySlug;
        //
        //         if (categorySlug == "")
        //         {
        //                 ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.Products.Count() / pageSize);
        //
        //                 return View(await _context.Products.OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToListAsync());
        //         }
        //
        //         Category category = await _context.Categories.Where(c => c.Slug == categorySlug).FirstOrDefaultAsync();
        //         if (category == null) return RedirectToAction("Index");
        //
        // var productsByCategory = _context.Products.Where(p => p.CategoryId == category.Id);
        // ViewBag.TotalPages = (int)Math.Ceiling((decimal)productsByCategory.Count() / pageSize);
        //
        // return View(await productsByCategory.OrderByDescending(p => p.Id).Skip((p - 1) * pageSize).Take(pageSize).ToListAsync());
        // }
    }
}