namespace Cookbook.Models
{
    public class RecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public FileManagerModel? FileManager { get; set; }

        public RecipeViewModel()
        {

        }

        public RecipeViewModel(Recipe recipe, FileManagerModel fileManager)
        {
            this.Recipe = recipe;
            this.FileManager = fileManager;
        }
    }
}
