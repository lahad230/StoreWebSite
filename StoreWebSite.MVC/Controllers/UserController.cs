using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Helpers;
using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.Models;

namespace StoreWebSite.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly ICartIManagement _cartManagement;
        private readonly IUnitOfWork _db;
        private readonly IUserManagement _um;
        private readonly IItemInCarts _itemInCarts;
        private readonly IProductsManagement _productsManagement;

        public UserController(ICartIManagement cart, IUnitOfWork db, IUserManagement um, IItemInCarts itemInCarts, IProductsManagement productsManagement)
        {
            _productsManagement = productsManagement;
            _itemInCarts = itemInCarts;
            _cartManagement = cart;
            _db = db;
            _um = um;
        }

        //view for updating user details.
        public IActionResult CreateOrUpdate()
        {
            var currentUser = _um.GetUser();
            if (currentUser != null)
            {
                //if not new user, clear password field before showing view.
                currentUser.Password = null;
                var vm = new CreateOrUpdateViewModel(currentUser);
                
                return View(vm);
            }
            return View();
        }


        //executes the creation of a user and saving it to database.
        public IActionResult CreateUpdateResult(CreateOrUpdateViewModel vm)
        {
            //flag to show in view if saving succeded.
            ViewBag.Success = true;

            //flag to show if new user was created, or a pre existed user was just updated.
            ViewBag.NewUser = true;

            //this set up an if() that will stop _login view validation to show up
            //when a guest try to register using this action.
            TempData["Validation"] = "true";

            //get current user.
            var currentUser = _um.GetUser();

            //if user exist, remove username from validation checks.
            if (currentUser != null)
            {
                ModelState.Remove("UserName");
            }

            //returns to CreateOrUpdate view if form was not completed.
            if (!ModelState.IsValid)
            {
                //check each property, on faild validation create Tempdata with the propery name
                //as the key.
                foreach (var item in ModelState.Keys)
                {                    
                    if (ModelState.GetValidationState(item) == ModelValidationState.Invalid)
                    {
                        //this will be used to check which properties failed validation
                        //ad a span with red asterisk will be created in the view besides the
                        //relevent input element.
                        TempData[item] = "Show*";
                    }
                    
                }
                return View("CreateOrUpdate", vm);
            }

            //Create or update User in db.
            if (currentUser != null)
            {
                //Updating an existing user.
                ViewBag.NewUser = false;

                var userToUpdate = _db.UserRepository.Get(currentUser.Id);

                //encrypts the new password.
                vm.Password = EncryptionHelper.ComputeHash(vm.Password, "SHA512", null);
                _um.UpdateUser(userToUpdate, vm);

                //tries to save data to database.
                if (!_db.Save())
                {
                    //if failed in saving, show that in the view.
                    ViewBag.Success = false;
                }
            }
            else
            {
                //creats a User class from the viewmodel sent in the form.
                var user = _um.CreateNewUserFromViewModel(vm);

                //encrypts the password.
                user.Password = EncryptionHelper.ComputeHash(user.Password, "SHA512", null);

                //tires to save user in the database.
                if (!_db.UserRepository.Create(user))
                {
                    //if failed in saving, show that in the view.
                    ViewBag.Success = false;
                }
                else
                {
                    //Creatin a new user.
                    ViewBag.NewUser = true;

                    //after saving, create a new log entry in the database.
                    _um.CreateCurrentUserCookie(user);
                    LogEntry entry = new LogEntry()
                    {
                        Content = $"user: {user.UserName} created",
                        TimeStamp = DateTime.Now
                    };
                    _db.LogEntryRepository.Create(entry);
                }
            }

            return View(vm);
        }

        //pratial view(_Login) func for login.
        public IActionResult Login(LoginViewModel vm)
        {
            //if form is not completed, send model back.
            if (!ModelState.IsValid)
            {
                View("_Login", vm);
            }
            else
            {
                //get user form database.
                var user = _db.UserRepository.GetByUserName(vm.UserName);
                if (user != null)
                {
                    //try to decrypt password.
                    if (!EncryptionHelper.VerifyHash(vm.Password, "SHA512", user.Password))
                    {
                        //decryption failed, username and password does not match.
                        //raise a flag to reveal a warning.
                        //send model back to view.
                        ViewBag.WrongInput = true;
                        View("_Login", vm);
                    }
                    else
                    {
                        //username and password match.
                        //log in the user.
                        _um.CreateCurrentUserCookie(user);
                    }
                }
                else
                {
                    ViewBag.WrongInput = true;
                    View("_Login", vm);
                }
            }
            //redirect to main site view.
            return View("~/views/Product/Index.cshtml", _productsManagement.GetProducts());
        }


        //action for Disconnect button in "_WelcomeUser" partial view.
        public IActionResult Disconnect()
        {
            // func for loging out.
            _um.DeleteUserCookieAndSession();
            //redirect to main site view.
            return View("~/views/Product/Index.cshtml", _productsManagement.GetProducts());
        }
    }
}
