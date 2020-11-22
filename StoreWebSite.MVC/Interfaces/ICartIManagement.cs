using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Models;
using StoreWebSite.MVC.ServiceModels;
using System;
using System.Collections.Generic;

namespace StoreWebSite.MVC.Interfaces
{
    public interface ICartIManagement
    {

        //this func will clear items for the list of all items in carts, when the cookie itself will be deleted
        //based on the value in "TimeToDelete" propery.
        void ClearOldItems();

        //clears all the items in the user cart, and removes said items from the list of all items in carts.
        void ClearUserData();

        //adds item to user cart, and update the deletion time of items in user cart in list of all items in cart
        //(to keep the deletion of the cookie with the removal of items in list of all items in carts synced).
        void SetData(CartItem cartProduct);

        //gets all items in user cart.
        List<Product> ShowItems();

        //set products as bought in db, removes items from list opf all items in carts and deleted user cart cookie.
        bool BuyItems();

        //removes an item from user cart, and from list of all items in carts.
        void RemoveItem(int productId);



        //test method(see the notes in the service page)
        public void DeleteCookieOnFirstRun();

    }
}
