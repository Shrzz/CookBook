using Microsoft.AspNetCore.Identity;

namespace Cookbook.Models
{
    public class ChangeRoleModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public List<ApplicationRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleModel()
        {
            AllRoles = new List<ApplicationRole>();
            UserRoles = new List<string>();
        }
    }
}
