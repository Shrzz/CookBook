using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cookbook.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ApplicationUser? Author { get; set; }
        public List<string>? Steps { get; set; }
        public List<string>? Ingredients { get; set; }
        public List<Image>? Images { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
