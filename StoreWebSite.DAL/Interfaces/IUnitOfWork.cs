using StoreWebSite.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreWebSite.DAL.Interfaces
{
    //data access interface.
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        IProductRepository ProductsRepository { get; }

        IRepository<LogEntry> LogEntryRepository { get; }

        bool Save();
    }
}
