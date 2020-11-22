using Microsoft.AspNetCore.Http;
using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWebSite.MVC.Interfaces
{
    public interface IProductsManagement
    {
        //gets all the products that should be showen to the user.
        List<Product> GetProducts(bool isOrdered = false);

        //converts picture to array, to be able to save it in database.
        byte[] ConvertPicToByteArray(IFormFile Picture);

        //creates product from data in the view model.
        Product CreateNewProductFromViewModelAndUser(CreateAdViewModel vm, User owner);
    }
}
