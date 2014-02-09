using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newsfeed.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string AvatarPath { get; set; }

        //contains the objectId to username mapping of the blocked user.
        public Dictionary<string, string> BlockedUsers { get; set; }

        public bool RememberMe { get; set; }
    }
}