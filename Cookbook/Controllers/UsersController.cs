using Cookbook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cookbook.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        RoleManager<ApplicationRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index() 
        {
            return View(_userManager.Users.ToList());
        }

		[HttpGet]
		public async Task<IActionResult> Delete(string userId)
        {
			ApplicationUser user = await _userManager.FindByIdAsync(userId);
			if (user is null)
            {
				return NotFound();
            }

			await _userManager.DeleteAsync(user);
			return RedirectToAction("Index");
        }

        //roles

        [HttpGet]
        public IActionResult Roles()
        {
            return View(_roleManager.Roles.ToList());
        }

        [HttpGet]
		public async Task<IActionResult> EditRoles(string userId)
		{
			ApplicationUser user = await _userManager.FindByIdAsync(userId);

			if (user is null)
			{
				return NotFound(user);
			}

			var userRoles = await _userManager.GetRolesAsync(user);
			var allRoles = _roleManager.Roles.ToList();
			ChangeRoleModel model = new ChangeRoleModel
			{
				UserId = user.Id,
				UserEmail = user.Email,
				UserRoles = userRoles,
				AllRoles = allRoles
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditRoles(string userId, List<string> roles)
		{
			ApplicationUser user = await _userManager.FindByIdAsync(userId);
			if (user is null)
			{
				return NotFound();
			}

			var userRoles = await _userManager.GetRolesAsync(user);
			var addedRoles = roles.Except(userRoles);
			var removedRoles = userRoles.Except(roles);

			await _userManager.AddToRolesAsync(user, addedRoles);
			await _userManager.RemoveFromRolesAsync(user, removedRoles);

			return RedirectToAction("Index");
		}
    }
}
