using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreWebSite.MVC.Models;

namespace StoreWebSite.MVC.Controllers
{
    public class ErrorController : Controller
    {

        //if user tries to see detail of an item that doest exists, shows a special 404 message.
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404 && TempData["ProductError"] != null)
                {
                    return View("404");
                }
            }
            return View(new ErrorViewModel());
        }

    }
}
