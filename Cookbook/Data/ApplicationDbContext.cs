using Cookbook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Json;

namespace Cookbook.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Like> Likes { get; set; }

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Like>().
                HasKey(l => new { l.UserId, l.RecipeId });

            builder.Entity<Recipe>().Property(p => p.Steps).HasConversion(
                value => JsonConvert.SerializeObject(value, new JsonSerializerSettings()),
                value => JsonConvert.DeserializeObject<List<string>>(value, new JsonSerializerSettings())
            );

            builder.Entity<Recipe>().Property(p => p.Ingredients).HasConversion(
                value => JsonConvert.SerializeObject(value, new JsonSerializerSettings()),
                value => JsonConvert.DeserializeObject<List<string>>(value, new JsonSerializerSettings())
            );
        }
    }
}