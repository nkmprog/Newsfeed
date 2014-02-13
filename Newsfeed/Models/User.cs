using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newsfeed.Models
{
    public class User
    {
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name="Password")]
        public string Password { get; set; }

        [Display(Name= "Avatar")]
        public string AvatarPath { get; set; }

        [Display(Name="Blocked Users")]
        public IList<string> BlockedUsers { get; set; }

        public bool RememberMe { get; set; }
    }
}