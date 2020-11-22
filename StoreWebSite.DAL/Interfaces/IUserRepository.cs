using StoreWebSite.DAL.Models;

namespace StoreWebSite.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        //gets user by its user name property.
        User GetByUserName(string userName);
    }
}
