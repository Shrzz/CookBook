using Cookbook.Data.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cookbook.Models
{
    public class Recipe : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ApplicationUser? Author { get; set; }
        public List<string>? Steps { get; set; }
        public List<string>? Ingredients { get; set; }
        public string? ImagesDirectory { get; set; }
        public DateTime CreationTime { get; set; }

        public Recipe()
        {
            CreationTime = DateTime.Now;
        }

    }
}
