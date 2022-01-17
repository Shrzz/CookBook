using Cookbook.Data.Repositories;
using Cookbook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Cookbook.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private const int pageSize = 10;

        private readonly UserRepository _userRepository;
        private readonly RecipeRepository _recipeRepository;
        private readonly FileManager _fileManager;

        public RecipesController(RecipeRepository recipeRepository, UserRepository userRepository, FileManager fileManager)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _fileManager = fileManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {
            return View(GetSortedRecipesList(currentFilter, searchString, pageNumber));
        }

        [HttpGet]
        [Route("Recipes/User/")]
        public async Task<IActionResult> User(string currentFilter, string searchString, int? pageNumber)
        {
            var userId = int.Parse(base.User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(GetSortedRecipesList(currentFilter, searchString, pageNumber, userId));
        }

        [HttpGet]
        [Route("Recipes/User/{userId:int}")]
        public async Task<IActionResult> User(string currentFilter, string searchString, int userId, int? pageNumber)
        {
            return View(GetSortedRecipesList(currentFilter, searchString, pageNumber, userId));
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

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

                int userId = int.Parse(base.User.FindFirstValue(ClaimTypes.NameIdentifier));
                ApplicationUser user = _userRepository.GetById(userId);
                if (user is null)
                {
                    return NotFound();
                }

                rvm.Recipe.Author = user;

                string directoryName = _fileManager.GenerateDirectoryName(rvm.Recipe.Title);
                rvm.Recipe.ImagesDirectory = _fileManager.UploadFiles(_fileManager.GetDirectoryRelativePath(directoryName), rvm.FileManager);

                _recipeRepository.Create(rvm.Recipe);
                return RedirectToAction("User");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }

            return View(rvm);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var recipe = _recipeRepository.GetById(id, "Author");
            if (recipe is null)
            {
                return NotFound();
            }

            var rvm = new RecipeViewModel() { Recipe = recipe, FileManager = _fileManager.GetFileManagerModel(recipe.ImagesDirectory) };

            return View(rvm);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(RecipeViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                rvm.Recipe.ImagesDirectory = _fileManager.UploadFiles(rvm.Recipe.ImagesDirectory, rvm.FileManager);
                _recipeRepository.Update(rvm.Recipe);
            }

            return RedirectToAction("User");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var entityToRemove = _recipeRepository.GetById(id);
            _fileManager.RemoveDirectory(entityToRemove.ImagesDirectory);
            _recipeRepository.Delete(id);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult DeleteImage(string fileToRemove)
        {
            _fileManager.RemoveFile(fileToRemove);

            var a = Request.Headers["Referer"].ToString();
            return Redirect(a);
        }

        public async Task<PaginatedList<Recipe>> GetSortedRecipesListAsync(string currentFilter, string searchString, int? pageNumber, int? userId = null)
        {
            if (searchString != null)
            {
                pageNumber = 1;
                ViewData["CurrentFilter"] = searchString;
            }
            else
            {
                searchString = currentFilter;
            }

            var recipes = _recipeRepository.GetAll();
            if (userId != null)
            {
                recipes = recipes.Where(r => r.Author.Id == userId);
                ViewData["TargetUser"] = userId;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(r => r.Title.Contains(searchString));
            }

            return await PaginatedList<Recipe>.CreateAsync(recipes, pageNumber ?? 1, pageSize);
        }

        public PaginatedList<RecipeIndexModel> GetSortedRecipesList(string currentFilter, string searchString, int? pageNumber, int? userId = null)
        {
            if (searchString != null)
            {
                pageNumber = 1;
                ViewData["CurrentFilter"] = searchString;
            }
            else
            {
                searchString = currentFilter;
            }

            var recipes = _recipeRepository.GetAll("Author");
            if (userId != null)
            {
                recipes = recipes.Where(r => r.Author.Id == userId);
                ViewData["TargetUser"] = userId;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(r => r.Title.Contains(searchString));
            }

            var list = new List<RecipeIndexModel>();
            foreach (var item in recipes)
            {
                var r = new RecipeIndexModel(item.Id, item.Title, item.Description, item.CreationTime, item.Author.UserName, _fileManager.GetSingleImageFromDirectory(item.ImagesDirectory));
                list.Add(r);
            }

            return PaginatedList<RecipeIndexModel>.Create(list, pageNumber ?? 1, pageSize);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

