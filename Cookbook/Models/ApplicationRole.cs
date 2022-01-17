﻿using Microsoft.AspNetCore.Identity;

namespace Cookbook.Models
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole(string name) : base(name)
        {

        }
    }

}
