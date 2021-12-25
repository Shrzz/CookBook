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
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IRecipeRepository _recipeRepository;

        public RecipesController(IRecipeRepository recipeRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public IActionResult Images()
        {
            FileManagerModel model = new FileManagerModel();
            var userImagesPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/images/recipes");
            DirectoryInfo dir = new DirectoryInfo(userImagesPath);
            FileInfo[] files = dir.GetFiles();
            model.Files = files;
            return View(model);
        }

        public IActionResult DeleteImage(string fname)
        {
            string _imageToBeDeleted = $"{_hostingEnvironment.WebRootPath}/{fname}";
            if ((System.IO.File.Exists(fname)))
            {
                System.IO.File.Delete(fname);
            }

            var a = Request.Headers["Referer"].ToString();
            return Redirect(a);
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

            var rvm = new RecipeViewModel() { Recipe = recipe, FileManager = GetFileManager(recipe.ImagesDirectory) };
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

                var folderName = $"{DateTime.Now.Ticks}_{rvm.Recipe.Title.Replace(' ', '_')}";
                var relativePath = Path.Combine("/uploads", "images", "recipes", folderName);
                var fullFolderPath = Path.Combine($"{_hostingEnvironment.WebRootPath}/{relativePath}");

                if (!Directory.Exists(fullFolderPath))
                {
                    Directory.CreateDirectory(fullFolderPath);
                }
                DirectoryInfo dir = new DirectoryInfo(fullFolderPath);

                if (rvm.FileManager.IFormFiles is not null)
                {
                    foreach (var item in rvm.FileManager.IFormFiles)
                    {
                        var fullFilePath = Path.Combine(fullFolderPath, item.FileName);
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            System.IO.File.Delete(fullFilePath);
                        }

                        item.CopyTo(new FileStream(fullFilePath, FileMode.Create));
                    }
                }

                rvm.Recipe.ImagesDirectory = relativePath;
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

            var rvm = new RecipeViewModel() { Recipe = recipe, FileManager = GetFileManager(recipe.ImagesDirectory) };

            return View(rvm);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(RecipeViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                var fullFolderPath = $"{_hostingEnvironment.WebRootPath}/{rvm.Recipe.ImagesDirectory}";
                DirectoryInfo directory = new DirectoryInfo(fullFolderPath);
                foreach (var item in rvm.FileManager.IFormFiles)
                {
                    var fullFilePath = $"{fullFolderPath}/{item.FileName}";
                    item.CopyTo(new FileStream(fullFilePath, FileMode.Create));
                }

                _recipeRepository.Update(rvm.Recipe);
            }

            return RedirectToAction("MyRecipes");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _recipeRepository.Delete(id);
            return RedirectToAction("MyRecipes");
        }

        private FileManagerModel GetFileManager(string imagesDirectory)
        {
            string fullPath = $"{ _hostingEnvironment.WebRootPath}/{imagesDirectory}";
            FileManagerModel model = new FileManagerModel();
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            DirectoryInfo dir = new DirectoryInfo(fullPath);
            FileInfo[] files = dir.GetFiles();
            model.Files = files;

            return model;
        }
    }

}
