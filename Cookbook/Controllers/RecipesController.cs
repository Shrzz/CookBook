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

                List<string> images = new List<string>();
                if (rvm.Images is not null)
                {
                    var folderPath = $"/uploads/images/recipes/{DateTime.Now.Ticks}_{rvm.Title.Replace(' ', '_')}/";
                    var fullPath = $"{_hostingEnvironment.ContentRootPath}wwwroot/{folderPath}";
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    foreach (var item in rvm.Images)
                    {
                        var fileName = item.FileName.Replace(' ', '_');
                        var fullFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, fullPath, fileName);
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            System.IO.File.Delete(fullFilePath);
                        }

                        item.CopyTo(new FileStream(fullFilePath, FileMode.Create));

                        var filePath = Path.Combine(folderPath, fileName);
                        images.Add(filePath);
                    }
                }

                Recipe recipe = new Recipe() { Title = rvm.Title, Description = rvm.Description, Author = user, Ingredients = rvm.Ingredients, Steps = rvm.Steps, Images = images };

                _recipeRepository.Create(recipe);
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
