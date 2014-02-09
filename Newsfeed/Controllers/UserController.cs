using Newsfeed.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Newsfeed.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            var userManager = new UserManager();
            if (ModelState.IsValid)
            {
                if (userManager.IsValid(user.Username, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login information is incorrect.");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Models.User user)
        {
            var userManager = new UserManager();
            if (ModelState.IsValid)
            {
                //check if user already exists
                if (!userManager.IsRegistered(user.Username))
                {
                    userManager.RegisterUser(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The supplied username is already taken.");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return View();
        }
    }
}
