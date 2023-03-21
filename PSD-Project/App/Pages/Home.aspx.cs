using System;
using System.Collections.Generic;
using System.Web.UI;
using PSD_Project.API.Features.LogIn;
using PSD_Project.API.Features.Users;
using PSD_Project.API.Service;
using PSD_Project.App.Common;
using PSD_Project.App.Models;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Home : Page
    {
        private static readonly IAuthService AuthService = Services.GetAuthService();
        private static readonly IUsersService UsersService = Services.GetUsersService();

        protected UserRole CurrentUserRole = UserRole.Customer;
        protected List<User> Customers = new List<User>();
        protected List<User> Staff = new List<User>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Session.GetUserSession()
                .Match(
                    some: details =>
                    {
                        RoleLabel.Text = details.Role.Name;
                        var staff = UsersService.GetUsersWithRole(1);
                        var customers = UsersService.GetUsersWithRole(0);
                        CurrentUserRole = RoleById(details.Role.Id).Ok().OrElse(UserRole.Customer);
                        Customers = customers.Ok().OrElse(new List<User>());
                        Staff = staff.Ok().OrElse(new List<User>());
                    },
                    none: () => Response.Redirect("Login.aspx"));
        }
        
        private Try<UserRole, Exception> RoleById(int roleId)
        {
            switch (roleId)
            {
                case 0:
                    return Try.Of<UserRole, Exception>(UserRole.Customer);
                case 1:
                    return Try.Of<UserRole, Exception>(UserRole.Staff);
                case 2:
                    return Try.Of<UserRole, Exception>(UserRole.Admin);
                default:
                    return Try.Err<UserRole, Exception>(new ArgumentOutOfRangeException());
            }
        }
    }
}