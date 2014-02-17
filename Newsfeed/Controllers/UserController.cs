﻿using Newsfeed.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newsfeed.Domain;

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
        public ActionResult Register(Models.User user, HttpPostedFileBase avatar)
        {
            var userManager = new UserManager();
            var gridFS = new GridFSRepository();
            if (ModelState.IsValid)
            {
                //check if user already exists
                if (!userManager.IsRegistered(user.Username))
                {
                    user.AvatarId = gridFS.UploadFile(avatar.InputStream, avatar.FileName);
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
        
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit()
        {
            var username = HttpContext.User.Identity.Name;
            var userManager = new UserManager();
            var user = userManager.GetUserByUserName(username);
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(string newUserName, string userToBlock, string userToUnblock, HttpPostedFileBase avatar) 
        {
            var oldUserName = HttpContext.User.Identity.Name;
            var userManager = new UserManager();
            var user = userManager.GetUserByUserName(oldUserName);
            Boolean changed = false;
            if (newUserName != null && newUserName != "" && oldUserName != newUserName && !userManager.IsRegistered(newUserName))
            {
                // Change the user name and client information
                // Drop the old cookie and create a new one
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(newUserName, false);
                user.Username = newUserName;
                changed = true;
            }
            if (userToBlock != null && userToBlock != userToUnblock) 
            {
                if (userToBlock != oldUserName && userToBlock != newUserName && userManager.IsRegistered(userToBlock))
                {
                    user.BlockedUsers.Add(userToBlock);
                    changed = true;
                }

                if (user.BlockedUsers.Contains(userToUnblock))
                {
                    user.BlockedUsers.Remove(userToUnblock);
                    changed = true;
                }
            }
            if (avatar != null)
            {
                var gridFs = new GridFSRepository();
                gridFs.RemoveFile(user.AvatarId);
                user.AvatarId = gridFs.UploadFile(avatar.InputStream, avatar.FileName);
                changed = true;
            }
            if (changed == true)
            {
                userManager.SaveChanges(user);
            }
            
            return View(user);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Password() 
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Password(string newPassword, string newPasswordConfirmation)
        {
            var oldUserName = HttpContext.User.Identity.Name;
            var userManager = new UserManager();
            var user = userManager.GetUserByUserName(oldUserName);

            if (newPassword == newPasswordConfirmation && newPassword != user.Password)
            {
                user.Password = newPassword;
                userManager.SaveChanges(user);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Ratings()
        {
            var userManager = new UserManager();
            ViewBag.Ratings = userManager.generateUserRatings();
            return View();
        }
    }
}
