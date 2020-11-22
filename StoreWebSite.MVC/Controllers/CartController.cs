using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.Models;
using StoreWebSite.MVC.ServiceModels;

namespace StoreWebSite.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartIManagement _cartManagment;
        public CartController(ICartIManagement cm)
        {
            _cartManagment = cm;
        }

        //loads ShowCart view with all the items in current user's cart.
        public IActionResult ShowCart()
        {
            return View(_cartManagment.ShowItems());
        }

        //loads view after user buys his cart.
        public IActionResult BuyCart()
        {            
            return View("Receipt", _cartManagment.BuyItems());
        }


        //redirects to main view after updating cart cookie.
        public IActionResult AddToCart(CartItem cartItem)
        {
            _cartManagment.SetData(cartItem);
            return RedirectToAction("Index", "Product");
        }

        //redirects to ShowCart after removing an item from cart.
        public IActionResult RemoveFromCart(int productId)
        {
            _cartManagment.RemoveItem(productId);
            return RedirectToAction("ShowCart");
        }
    }
}
