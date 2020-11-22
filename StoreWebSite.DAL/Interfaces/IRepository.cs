using System;
using System.Collections.Generic;
using System.Text;

namespace StoreWebSite.DAL.Interfaces
{
    //main func to get data from repositories.
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get();

        T Get(int id);

        bool Create(T entity);

    }
}
