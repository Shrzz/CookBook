using Cookbook.Data;
using Cookbook.Data.Repositories;
using Cookbook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Cookbook.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecipeRepository _recipeRepository;

        public RecipesController(IRecipeRepository recipeRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View(_recipeRepository.GetAll());
        }

        [HttpGet]
        public IActionResult MyRecipes()
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = _recipeRepository.GetAllCreatedByUser(id);
            return View(list);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var recipe = _recipeRepository.GetById(id);
            if (recipe is null)
            {
                return NotFound();
            }

            return View(recipe);
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Recipe recipe)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ApplicationUser user = _userRepository.GetById(userId);
                if (user is null)
                {
                    return NotFound();
                }

                recipe.Author = user;

                if (ModelState.IsValid)
                {
                    _recipeRepository.Create(recipe);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }

            return View(recipe);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var recipe = _recipeRepository.GetById(id);
            if (recipe is null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _recipeRepository.Update(recipe);
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _recipeRepository.Delete(id);
            var a = Request.Headers["Referer"].ToString();
            return Redirect(a);
        }

    }
}
