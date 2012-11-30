﻿using System;
using System.Configuration;
using System.Web.Mvc;
using ErrorGun.Web.Filters;
using ErrorGun.Web.Models;

namespace ErrorGun.Web.Controllers
{
    [ForwardAwareRequireHttps]
    public class AdminController : Controller
    {
        private static readonly string _AdminPassword = ConfigurationManager.AppSettings["AdminPassword"];

        public ActionResult Index(string password)
        {
            bool passwordCorrect = (password == _AdminPassword);
            return View(new AdminModel(passwordCorrect));
        }

    }
}
