using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWebSite.MVC.Services
{
    //list of all items in cart.
    //items in this list will not be shown to the user.
    public class ItemInCarts : IItemInCarts 
    {
        public LinkedList<CartItem> ItemsList { get; set; }
        public ItemInCarts()
        {
            ItemsList = new LinkedList<CartItem>();
        }
    }
}
