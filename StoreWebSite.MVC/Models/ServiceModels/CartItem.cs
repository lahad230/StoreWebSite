using System;

namespace StoreWebSite.MVC.ServiceModels
{
    //model for items in the list of items in all carts.
    public class CartItem
    {
        public int ProductId { get; set; }

        public DateTime Added { get; set; }
    }
}
