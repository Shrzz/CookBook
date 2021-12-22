namespace Cookbook.Models
{
    public class RecipeViewModel
    {
        public Recipe? Recipe { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
