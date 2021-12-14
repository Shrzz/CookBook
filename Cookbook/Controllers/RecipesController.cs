using Cookbook.Data;
using Cookbook.Data.Repositories;
using Cookbook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cookbook.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeRepository _repository;

        public RecipesController(IRecipeRepository repository)
        {
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
            if (recipe != null)
            {
                return View(recipe);
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
