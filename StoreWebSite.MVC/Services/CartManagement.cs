using StoreWebSite.MVC.Interfaces;
using System;
using System.Collections.Generic;
using StoreWebSite.MVC.ServiceModels;
using Microsoft.AspNetCore.Http;
using StoreWebSite.MVC.Models;
using System.Linq;
using System.Text;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.DAL.Models;

namespace StoreWebSite.MVC.Services
{
    //needs refractoring
    public class CartManagement : ICartIManagement
    {
        private readonly IItemInCarts _itemInCarts;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUnitOfWork _db;

        //name for cart cookie.
        private readonly string _cookieName = "MyCart";

        //delimiter to seperate items in cart cookie.
        private readonly string _splitDelimiter = " ";

        //value used to sync deletion of cart cookie and clear items from list of all items in cart.
        private readonly TimeSpan _timeToDelete = TimeSpan.FromHours(1);

        public CartManagement(IHttpContextAccessor httpContext, IItemInCarts itemInCarts, IUnitOfWork db)
        {
            _db = db;
            _itemInCarts = itemInCarts;
            _httpContext = httpContext;
        }

        //removes old items(items in deleted cookies) from list of all items in cart.
        public void ClearOldItems()
        {
            LinkedList<CartItem> toRemove = new LinkedList<CartItem>();
            //for aall the list, check age of item. 
            foreach (CartItem item in _itemInCarts.ItemsList)
            {
                if (DateTime.Now.Subtract(item.Added) > _timeToDelete)
                {
                    toRemove.AddFirst(item);
                }
            }

            //remove old items.
            foreach (CartItem item in toRemove)
            {
                _itemInCarts.ItemsList.Remove(item);
            }
        }

        //removes items in user cart from list of all items in cart, then deletes the cart cookie.
        public void ClearUserData()
        {
            //get items id by spliting the cookie string in the delimiter.
            string[] cartItemsStringSplit = SplitData();
            if (cartItemsStringSplit != null)
            {
                foreach (var cookieItem in cartItemsStringSplit)
                {
                    var itemToRemove = _itemInCarts.ItemsList.FirstOrDefault(item => item.ProductId == int.Parse(cookieItem));
                    if (itemToRemove != null)
                    {
                        _itemInCarts.ItemsList.Remove(itemToRemove);
                    }
                }
                _httpContext.HttpContext.Response.Cookies.Delete(_cookieName);
            }
        }

        //Add item to cart cookie and to the list of all items in carts.
        //for other items in the users's cart, updates the removal time in the list of all items in carts.
        //(updating all the other items keeps the deletion of the cookie and the removal of all the relevent items
        // in the list of all items in carts synced).
        public void SetData(CartItem cartProduct)
        {
            //add item to the list of all items in carts.
            _itemInCarts.ItemsList.AddLast(cartProduct);

            //set cookie option to be removed in sync with items in list of items in carts.
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now + _timeToDelete
            };

            //create cookie if needed.
            if (_httpContext.HttpContext.Request.Cookies[_cookieName] == null)
            {
                _httpContext.HttpContext.Response.Cookies.Append(_cookieName, cartProduct.ProductId.ToString(), cookieOptions);
            }

            //update cookie if needed.
            else
            {
                //this will make sure cookie and items in cart collection will be erased at the same time.
                string cartItems = _httpContext.HttpContext.Request.Cookies[_cookieName];

                //split cookie string by delimiter.
                string[] cartItemsStringSplit = cartItems.Split(_splitDelimiter);

                //update the age of all the other items in the cart
                //to keep everything synced.
                foreach (var cookieItem in cartItemsStringSplit)
                {
                    foreach (var cartItem in _itemInCarts.ItemsList)
                    {
                        if (cartItem.ProductId == int.Parse(cookieItem))
                        {
                            cartItem.Added = DateTime.Now;
                        }
                    }
                }
                //add new item id to the rest of the cookie string.
                _httpContext.HttpContext.Response.Cookies.Append(_cookieName, cartItems + _splitDelimiter + cartProduct.ProductId.ToString(), cookieOptions);
            }
        }

        //show items in current user/guest cart.
        public List<Product> ShowItems()
        {
            var myCartProducts = new List<Product>();

            //get items ids from cookie string.
            string[] cartItemsStringSplit = SplitData();

            //get items from database based on their ids.
            if (cartItemsStringSplit != null)
            {
                foreach (string item in cartItemsStringSplit)
                {
                    var product = _db.ProductsRepository.Get(int.Parse(item));
                    myCartProducts.Add(product);
                }
            }
            return myCartProducts;
        }

        //removes an item for user/guest cart.
        public void RemoveItem(int productId)
        {
            CartItem toRemove = default;
            StringBuilder sb = new StringBuilder();

            //get items id from cookie string.
            string[] productsIdsSplit = SplitData();

            foreach (var item in productsIdsSplit)
            {
                //find the relevent item in the cookie.
                if (item == productId.ToString())
                {
                    foreach (CartItem cartItem in _itemInCarts.ItemsList)
                    {
                        //find the relevent item in the list of all items in carts.
                        if (cartItem.ProductId == productId)
                        {
                            //mark item to remove
                            //(cant be done inside the foreach)
                            toRemove = cartItem;
                            break;
                        }
                    }
                }
                //other items id are added to anew string.
                else
                {
                    sb.Append(item + _splitDelimiter);
                }
            }
            //now remove relevent item from list of all items in carts.
            _itemInCarts.ItemsList.Remove(toRemove);

            //if cart had more than 1 item
            //update the cookie string with the rest of the items.
            if (sb.ToString() != string.Empty)
            {
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now + _timeToDelete
                };
                _httpContext.HttpContext.Response.Cookies.Append(_cookieName, sb.ToString().TrimEnd(), cookieOptions);
            }
            //if cookie had just 1 item, delete it.
            else
            {
                _httpContext.HttpContext.Response.Cookies.Delete(_cookieName);
            }
        }

        //deletes cookie, updates Products in database, 
        //and removes relevent items from list of all items in carts.
        public bool BuyItems()
        {
            StringBuilder sb = new StringBuilder();

            //get items from cookie string.
            string[] productsIdsSplit = SplitData();
            if (productsIdsSplit != null)
            {
                foreach (var item in productsIdsSplit)
                {
                    //update relevent products in database.
                    var productToBuy = _db.ProductsRepository.Get(int.Parse(item));
                    productToBuy.IsPurchased = true;
                }

                //if database is updated.
                if (_db.Save())
                {
                    //create a new log entry for this action.
                    LogEntry entry = new LogEntry()
                    {
                        TimeStamp = DateTime.Now
                    };
                    foreach (var item in productsIdsSplit)
                    {
                        sb.Append(", " + item);
                    }
                    entry.Content = $"Products: {sb} were bought";
                    _db.LogEntryRepository.Create(entry);

                    //delete cart cookie and remove all relevent items from list of all items in carts.
                    ClearUserData();
                    return true;
                }
            }
            //if cart was empty, return false to show relevent message.
            return false;
        }

        //get items ids from cart cookie by spliting it in the delimiter.
        private string[] SplitData()
        {
            string cartItems = _httpContext.HttpContext.Request.Cookies[_cookieName];
            if (cartItems != null)
            {
                return cartItems.Split(_splitDelimiter);
            }
            return null;
        }


        //This is a method for running the site in a test environment.
        //The cookie expires after a set amount of time and so are the items in the list
        //of all items in carts, that means that if the site goes down, after that time passes
        //all the cookies will be deleted from users browsers and the list of all items in carts
        //will be empty as well. That makes sure that there will be a sync between what is in the users
        //carts and what products can be shown. 
        //because in testing the site will be dropped a lot of times without giving the time for the cookies
        //to expire, a situation may occure that a cookie with items will still exist but the list of all
        //items in carts will be empty.
        //This function serves to deal with this by deleting said cookie after dropping the site, but
        //before the time for the cookie to expire passes.
        //This method serves to make sure this site reacts as it should while you check my sites functionality.


        //THERE IS A WAY TO DEAL WITH THIS IN REAL LIFE, JUST WAITING THE ALOTED TIME BEFORE RAISING BACK THE SITE
        //WILL MAKE SURE THAT THE SYNC BETWEEN THE COOKIES AND THE LIST WILL REMAIN.
        //THE TIME CAN BE SET USING THE VARIABLE '_timeToDelete' IN THIS SERVICE.

        //(I didnt want to backup everything in the db, so that is why i am using this solution.
        //the instructions hinted towards a singleton service in which the items currently in all cart will
        //be saved, so saving it also in the db is a bit redundent)

        public void DeleteCookieOnFirstRun()
        {
            if(_itemInCarts.ItemsList.Count == 0 && _httpContext.HttpContext.Request.Cookies[_cookieName] != null)
            {
                _httpContext.HttpContext.Response.Cookies.Delete(_cookieName);
            }
        }
    }
}
