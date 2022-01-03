using Cookbook.Data.Repositories;
using Cookbook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cookbook.Controllers
{
    public class RecipesController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RecipeRepository _recipeRepository;
        private readonly FileManager _fileManager;

        public RecipesController(RecipeRepository recipeRepository, UserRepository userRepository, UserManager<ApplicationUser> userManager, FileManager fileManager)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _fileManager = fileManager;
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
            var recipe = _recipeRepository.GetById(id, "Author");
            if (recipe is null)
            {
                return NotFound();
            }

            var rvm = new RecipeViewModel() { Recipe = recipe, FileManager = _fileManager.GetFileManagerModel(recipe.ImagesDirectory) };
            return View(rvm);
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
        public IActionResult Create(RecipeViewModel rvm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ApplicationUser user = _userRepository.GetById(userId);

                if (user is null)
                {
                    return NotFound();
                }

                rvm.Recipe.Author = user;

                string directoryName = _fileManager.GenerateDirectoryName(rvm.Recipe.Title);
                rvm.Recipe.ImagesDirectory = _fileManager.UploadFiles(_fileManager.GetDirectoryRelativePath(directoryName), rvm.FileManager);

                _recipeRepository.Create(rvm.Recipe);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }

            return View(rvm);
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

            var rvm = new RecipeViewModel() { Recipe = recipe, FileManager = _fileManager.GetFileManagerModel(recipe.ImagesDirectory) };

            return View(rvm);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(RecipeViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                rvm.Recipe.ImagesDirectory = _fileManager.UploadFiles(rvm.Recipe.ImagesDirectory, rvm.FileManager);
                _recipeRepository.Update(rvm.Recipe);
            }

            return RedirectToAction("MyRecipes");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var entityToRemove = _recipeRepository.GetById(id);
            _fileManager.RemoveDirectory(entityToRemove.ImagesDirectory);
            _recipeRepository.Delete(id);
            return RedirectToAction("MyRecipes");
        }

        public IActionResult DeleteImage(string fileToRemove)
        {
            _fileManager.RemoveFile(fileToRemove);

            var a = Request.Headers["Referer"].ToString();
            return Redirect(a);
        }
    }
}

