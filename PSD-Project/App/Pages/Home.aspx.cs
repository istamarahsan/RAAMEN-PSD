using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Org.BouncyCastle.Ocsp;
using PSD_Project.API.Features.Authentication;
using PSD_Project.App.Common;
using PSD_Project.App.Services.Users;

namespace PSD_Project.App.Pages
{
    public partial class Home : Page
    {
        protected class UserDetails
        {
            public string Username;
            public string Email;
            public string Rolename;
        }
        protected UserDetails CurrentUser;
        protected List<UserDetails> Customers;
        protected List<UserDetails> Staff;
        private readonly IUserService userService = AppServices.Singletons.UserService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            Session.GetUserSession().Match(
                some: InitPage,
                none: () => Response.Redirect("Login.aspx"));
        }

        private void InitPage(UserSessionDetails userSessionDetails)
        {
            CurrentUser = new UserDetails
            {
                Username = userSessionDetails.Username,
                Email = userSessionDetails.Email,
                Rolename = userSessionDetails.Role.Name
            };
            if (userSessionDetails.Role.Name == "Staff" || userSessionDetails.Role.Name == "Admin")
            {
                Customers = userService.GetCustomers(Request.GetTokenFromCookie().Unwrap())
                    .Unwrap()
                    .Select(u => new UserDetails
                    {
                        Email = u.Email,
                        Rolename = u.Role.Name,
                        Username = u.Username
                    })
                    .ToList();
            }

            if (userSessionDetails.Role.Name == "Admin")
            {
                Staff = userService.GetStaff(Request.GetTokenFromCookie().Unwrap())
                    .Unwrap()
                    .Select(u => new UserDetails
                    {
                        Email = u.Email,
                        Rolename = u.Role.Name,
                        Username = u.Username
                    })
                    .ToList();
            }
        }
        
        
    }
}