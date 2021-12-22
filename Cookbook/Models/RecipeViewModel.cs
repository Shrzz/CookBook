namespace Cookbook.Models
{
    public class RecipeViewModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Steps { get; set; }
        public List<string>? Ingredients { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
