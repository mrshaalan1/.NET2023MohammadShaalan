using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;
using System.ComponentModel.DataAnnotations;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]

    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        private readonly DbContext context;

        public RolesController(RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult Index() => View(roleManager.Roles);

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([MinLength(2), Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {

                    TempData["Success"] = "The role has been created";
                    return RedirectToAction("Index");
                }
                else
                {


                    foreach (IdentityError error in result.Errors) ModelState.
                            AddModelError("", error.Description);
                }

            }
            ModelState.AddModelError("", "Minimum length is 2");

            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonmembers = new List<AppUser>();

            foreach (AppUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleEdit roleEdit)
        {
            IdentityResult result;

            foreach (string userId in roleEdit.AddIds ?? new string[] { })
            {
                AppUser appUser = await userManager.FindByIdAsync(userId);
                result = await userManager.AddToRoleAsync(appUser, roleEdit.RoleName);

            }

            foreach (string userId in roleEdit.DeleteIds ?? new string[] { })
            {
                AppUser appUser = await userManager.FindByIdAsync(userId);
                result = await userManager.RemoveFromRoleAsync(appUser, roleEdit.RoleName);
            }

            return Redirect(Request.Headers["Referer"].ToString());


        }


        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "The role does not exist";
            }
            else
            {
                IdentityResult result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    TempData["Success"] = "The role has been deleted";
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return RedirectToAction("Index");
        }




    }
}
