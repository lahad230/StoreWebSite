using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWebSite.MVC.Interfaces
{
    public interface IUserManagement
    {
        //gets current user.
        User GetUser();

        //create a cookie to remember logged in users.
        void CreateCurrentUserCookie(User user);

        //remove data remebering user (use in diconnect).
        public void DeleteUserCookieAndSession();

        //create a session to remember logged in users.
        void CreateCurrentUserSession(User user);

        //update User model with data from viewmodel.
        void UpdateUser(User userToUpdate, CreateOrUpdateViewModel vm);

        //creates User model from data in viewmodel.
        User CreateNewUserFromViewModel(CreateOrUpdateViewModel vm);
    }
}
