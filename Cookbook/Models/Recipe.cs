using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cookbook.Models
{
    public class Recipe
    {
        private string _stepsString;
        private string _ingridientString;

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ApplicationUser? Author { get; set; }
        public List<string>? Steps { get; set; }
        public List<string>? Ingridients { get; set; }
    }
}
