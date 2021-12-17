using System.ComponentModel.DataAnnotations;

namespace Cookbook.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ApplicationUser? Author { get; set; }

        public List<string>? Steps;
        public List<string>? Ingridients;
        
    }
}
