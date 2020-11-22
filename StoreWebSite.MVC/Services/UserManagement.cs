using Microsoft.AspNetCore.Http;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.DAL.Models;
using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.Models;
using StoreWebSite.MVC.SessionExtensions;
using System;

namespace StoreWebSite.MVC.Services
{
    public class UserManagement : IUserManagement
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUnitOfWork _db;
        private readonly ICartIManagement _cm;

        //name for session that hold logged in user.
        private string SessionName = "CurrentUser";

        //name for the flag session.
        private string SessionFlag = "NewSession";
        public UserManagement(IHttpContextAccessor httpContext, IUnitOfWork db, ICartIManagement cm)
        {
            _httpContext = httpContext;
            _db = db;
            _cm = cm;
        }


        //this func is used to check if a user is logged in to the site.
        //it checks for a session or cookie, if there is a cookie, a session with the user
        //will be created. this func is called everywhere checking for active user is needed.
        public User GetUser()
        {
            //if there is session return user.
            var sessionUser = _httpContext.HttpContext.Session.GetComplexData<User>(SessionName);
            if (sessionUser != null)
            {
                return sessionUser;
            }

            //if there is cookie, create session. (unless user decieded to disconnect)
            var cookieUserName = _httpContext.HttpContext.Request.Cookies[SessionName];
            User user = null;   
            
            //only create session if the "session" is null
            //it means the the user enterd the site with a cookie and
            //was not trying to disconnect(why he had no session.)
            if (cookieUserName != null && _httpContext.HttpContext.Session.GetString(SessionFlag) == null)
            {
                user = _db.UserRepository.GetByUserName(cookieUserName);
                CreateCurrentUserSession(user);
            }
            return user;
        }

        //func to create a session and cookie for user when he logs in.
        public void CreateCurrentUserCookie(User user)
        {
            CreateCurrentUserSession(user);
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.MaxValue
            };
            _httpContext.HttpContext.Response.Cookies.Append(SessionName, user.UserName, cookieOptions);
        }


        //main func for disconnecting user from site.
        public void DeleteUserCookieAndSession()
        {
            //this session creation serves as a check so method:
            //GetUser() will not create new session instance, 
            //and will allow for user to disconnect properly.
            _httpContext.HttpContext.Session.SetString(SessionFlag, "false");


            //if user disconnects, clear his cart data, and delete his cookie and session.
            _cm.ClearUserData();
            _httpContext.HttpContext.Session.Remove(SessionName);
            _httpContext.HttpContext.Response.Cookies.Delete(SessionName);
        }


        //creates a session with the active user details
        public void CreateCurrentUserSession(User user)
        {
            _httpContext.HttpContext.Session.SetComplexData(SessionName, user);
        }

        //change user prop[erties by those in the viewmodel.
        public void UpdateUser(User userToUpdate, CreateOrUpdateViewModel vm)
        {
            userToUpdate.FirstName = vm.FirstName;
            userToUpdate.LastName = vm.LastName;
            userToUpdate.BirthDay = vm.BirthDay;
            userToUpdate.Email = vm.Email;
            userToUpdate.Password = vm.Password;
        }

        //create new user, with data from viewmodel.
        public User CreateNewUserFromViewModel(CreateOrUpdateViewModel vm)
        {
            var user = new User()
            {
                UserName = vm.UserName
            };
            UpdateUser(user, vm);
            return user;
        }
    }
}
