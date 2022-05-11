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
        private readonly LikeRepository _likeRepository;
        private readonly FileManager _fileManager;

        public RecipesController(RecipeRepository recipeRepository, UserRepository userRepository, LikeRepository likeRepository, FileManager fileManager)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _likeRepository = likeRepository;
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
            var user = GetCurrentUser();

            return View(GetSortedRecipesList(currentFilter, searchString, pageNumber, user.Id));
        }

        [HttpGet]
        [Route("Recipes/User/{userId:int}")]
        public async Task<IActionResult> User(string currentFilter, string searchString, int userId, int? pageNumber)
        {
            return View(GetSortedRecipesList(currentFilter, searchString, pageNumber, userId));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
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
        public async Task<IActionResult> Liked(string currentFilter, string searchString, int? pageNumber)
        {
            var user = GetCurrentUser();

            return View(GetSortedRecipesList(currentFilter, searchString, pageNumber, user.Id, true));
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

                var user = GetCurrentUser();

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

        public PaginatedList<RecipeIndexModel> GetSortedRecipesList(string currentFilter, string searchString, int? pageNumber, int? userId = null, bool likedOnly = false)
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
                if (likedOnly)
                {
                    var likedByUser = _likeRepository.GetAllUserLiked((int)userId);
                    recipes = recipes.Join(likedByUser, r => r.Id, l => l.RecipeId, (r, l) => r);
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(r => r.Title.Contains(searchString));
            }

            var user = GetCurrentUser();

            var list = new List<RecipeIndexModel>();
            foreach (var recipe in recipes)
            {
                bool isLiked = _likeRepository.IsLikedByUser(user.Id, recipe.Id);
                var image = _fileManager.GetSingleImageFromDirectory(recipe.ImagesDirectory);
                var r = new RecipeIndexModel(recipe.Id, recipe.Title, recipe.Description, recipe.CreationTime, recipe.Author.UserName, image, isLiked);
                list.Add(r);
            }

            return PaginatedList<RecipeIndexModel>.Create(list, pageNumber ?? 1, pageSize);
        }

        public async Task Like(int recipeId)
        {
            var user = GetCurrentUser();
            Recipe recipe = _recipeRepository.GetById(recipeId);
            var like = new Like(user, recipe);

            if (_likeRepository.IsLikedByUser(user.Id, recipeId))
            {
                _likeRepository.Delete(user.Id, recipe.Id);
            }
            else
            {
                _likeRepository.Create(like);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private ApplicationUser GetCurrentUser()
        {
            int userId = int.Parse(base.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ApplicationUser user = _userRepository.GetById(userId);

            return user;
        }
    }
}

