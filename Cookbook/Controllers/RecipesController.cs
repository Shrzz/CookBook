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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecipeRepository _repository;

        public RecipesController(IRecipeRepository repository, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_repository.GetRecipes());
        } 

        [HttpGet]
        public IActionResult Details(int id)
        {
            var recipe = _repository.GetRecipe(id);
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
                
                //recipe.AuthorId = User.FindFirst(ClaimTypes.NameIdentifier).Value; 
                if (ModelState.IsValid)
                {
                    _repository.CreateRecipe(recipe);
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
            var recipe = _repository.GetRecipe(id);
            if (recipe is null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        //[Authorize]
        //[HttpPost]
        //public IActionResult Edit(int id)
        //{
        //    var recipe = _repository.GetRecipe(id);
        //    if (recipe is null)
        //    {
        //        return NotFound();
        //    }

        //    return View();
        //}

        [Authorize]
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }



    }
}
