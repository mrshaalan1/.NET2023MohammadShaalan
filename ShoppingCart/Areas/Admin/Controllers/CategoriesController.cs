using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoriesController : Controller
    {

        private readonly DataContext context;

        public CategoriesController(DataContext context)
        {
            this.context = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await context.Categories.
                OrderBy(x => x.Name).ToListAsync());
        }

        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", " ");

                var slug = await context.Categories.FirstOrDefaultAsync(
                    x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The category already exists.");
                    return View(category);
                }

                context.Add(category);
                await context.SaveChangesAsync();


                TempData["Success"] = "The category has been added";




                return RedirectToAction("Index");
            }
            return View(category);
        }
        public async Task<IActionResult> Edit(long id)
        {
            Category category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Category editedCategory)
        {
            if (ModelState.IsValid)
            {
                // Find the existing category by id
                var existingCategory = await context.Categories.FindAsync(id);

                if (existingCategory == null)
                {
                    return NotFound();
                }

                // Update the properties of the existing category
                existingCategory.Name = editedCategory.Name;
                existingCategory.Slug = editedCategory.Name.ToLower().Replace(" ", "-");

                // Save the changes
                context.Update(existingCategory);
                await context.SaveChangesAsync();

                TempData["Success"] = "The Category has been edited";

                return RedirectToAction("Index");
            }

            return View(editedCategory);
        }




        public async Task<IActionResult> Delete(long id)
        {
            Category category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["Error"] = "The category does not exist";
            }
            else
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                TempData["Success"] = "The category has been deleted";
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var pageID in id)
            {

                Category category = await context.Categories.FindAsync(pageID);
                context.Update(category);
                await context.SaveChangesAsync();
                count++;


            }

            return Ok();

        }
    }
}
