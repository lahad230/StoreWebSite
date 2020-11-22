using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.Models;

namespace StoreWebSite.MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _db;
        private readonly IUserManagement _um;
        private readonly IProductsManagement _productsManagement;

        public ProductController(IUnitOfWork db, IUserManagement um, IProductsManagement productsManagement)
        {
            _um = um;
            _db = db;
            _productsManagement = productsManagement;
        }

        //sits's main view, shows all currentl available products.
        public IActionResult Index(bool isOrdered = false)
        {
            return View(_productsManagement.GetProducts(isOrdered));
        }

        //shows CreateAd view.
        public IActionResult CreateAd()
        {
            return View();
        }

        //saves product to database and converting pics to arrays.
        [HttpPost]
        public IActionResult SaveAd(CreateAdViewModel vm, IFormFile Picture1, IFormFile Picture2, IFormFile Picture3)
        {
            //if model from form is not complete, redirects back to CreateAd view.
            if (!ModelState.IsValid)
            {
                return View("CreateAd", vm);
            }

            //converts pic to array.
            vm.Picture1 = _productsManagement.ConvertPicToByteArray(Picture1);

            vm.Picture2 = _productsManagement.ConvertPicToByteArray(Picture2);

            vm.Picture3 = _productsManagement.ConvertPicToByteArray(Picture3);

            //get the signed in user as the owner.
            var owner = _db.UserRepository.Get(_um.GetUser().Id);
           
            //translate viewmodel into database model.
            var product = _productsManagement.CreateNewProductFromViewModelAndUser(vm, owner);

            //tries to update database.
            if (!_db.ProductsRepository.Create(product))
            {
                //flag to change view if saving in the database fails.
                ViewBag.Success = false;
            }
            else
            {
                //create a new log entry for the newly created product.
                LogEntry log = new LogEntry()
                {
                    Content = $"Ad: {product.Title} was created by {owner.UserName}",
                    TimeStamp = DateTime.Now
                };
                _db.LogEntryRepository.Create(log);
                ViewBag.Success = true;
            }
            return View();
        }

        //show all the details of a certain product.
        public IActionResult ShowFullProductDetails(int productId, bool inCart = false)
        {
            //if you got to this view from the cart view, you'll be redirected back there by clicking "back".
            //otherwise you'll be redirected to main view.
            ViewBag.InCart = inCart;

            //get the product from database.
            var currnetProduct = _db.ProductsRepository.Get(productId);     
            
            //if product does't exist, show custom 404 message.
            if(currnetProduct == null)
            {
                TempData["ProductError"] = productId;
                return NotFound();
            }

            //show certain information about owner in the view.
            ViewBag.OwnerFirstName = currnetProduct.Owner.FirstName;
            ViewBag.OwnerLastName = currnetProduct.Owner.LastName;
            ViewBag.OwnerBirthday = currnetProduct.Owner.BirthDay;
            ViewBag.Email = currnetProduct.Owner.Email;          
            return View(currnetProduct);
        }        
    }
}
