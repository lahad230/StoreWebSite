using Microsoft.AspNetCore.Http;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWebSite.MVC.Services
{
    public class ProductManagement : IProductsManagement
    {
        private readonly ICartIManagement _cartManagement;
        private readonly IUnitOfWork _db;
        private readonly IItemInCarts _itemInCarts;
        public ProductManagement(ICartIManagement cartManagement, IUnitOfWork db, IItemInCarts itemInCarts)
        {
            _itemInCarts = itemInCarts;
            _cartManagement = cartManagement;
            _db = db;
        }

        //get all relevent products to show the user.
        public List<Product> GetProducts(bool isOrdered = false)
        {
            //test method(see notes in the CartManagement service)
            _cartManagement.DeleteCookieOnFirstRun();

            //check age of items in list of all items in carts
            //and remove old items from it.
            _cartManagement.ClearOldItems();

            List<Product> products = new List<Product>();
            IEnumerable<Product> productsDb;

            //get products from database base on the order needed.
            //get by date:
            if (isOrdered)
            {
                productsDb = _db.ProductsRepository.GetProductsByIsPurchased(false).OrderBy(prod => prod.Date);
            }
            //get by title:
            else
            {
                productsDb = _db.ProductsRepository.GetProductsByIsPurchased(false).OrderBy(prod => prod.Title);
            }

            //check which items are in the list of all in carts.
            //those items will not be shown to the user.
            foreach (var product in productsDb)
            {
                bool inCart = false;
                foreach (var item in _itemInCarts.ItemsList)
                {
                    if (product.Id == item.ProductId)
                    {
                        inCart = true;
                    }
                }
                if (!inCart)
                {
                    //all items not ine the list of items in all carts will be shown to user.
                    products.Add(product);
                }
            }
            return products;
        }

        //create new Product model from data in view model.
        public Product CreateNewProductFromViewModelAndUser(CreateAdViewModel vm, User owner)
        {
            var product = new Product()
            {
                Title = vm.Title,
                Owner = owner,
                ShortDescription = vm.ShortDescription,
                LongDescription = vm.LongDescription,
                Date = DateTime.Now,
                Price = vm.Price,
                Picture1 = vm.Picture1,
                Picture2 = vm.Picture2,
                Picture3 = vm.Picture3
            };

            return product;
        }

        //convert pic to array.
        public byte[] ConvertPicToByteArray(IFormFile Picture)
        {
            //only certain file types are accepted.
            if (Picture != null && (Picture.ContentType == "image/png" || Picture.ContentType == "image/jpeg" || Picture.ContentType == "image/bmp"))
            {
                using (var stream = new MemoryStream())
                {
                    Picture.CopyTo(stream);
                    return stream.ToArray();
                }
            }
            return null;
        }
    }
}
