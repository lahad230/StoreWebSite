using StoreWebSite.MVC.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWebSite.MVC.Interfaces
{
    //hold all the items currently in carts.
    public interface IItemInCarts
    {
        LinkedList<CartItem> ItemsList { get; set; }
    }
}
