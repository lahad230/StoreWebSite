using StoreWebSite.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreWebSite.DAL.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        //func to get bought or unbought products.
        IEnumerable<Product> GetProductsByIsPurchased(bool isPurchased);
    }
}
