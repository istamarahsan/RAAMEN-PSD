using System;
using System.Collections.Generic;
using System.Web.UI;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Common;
using PSD_Project.App.Models;
using PSD_Project.Services;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Home : Page
    {
        private static readonly IAuthService AuthService = new AuthService();
        private static readonly IUsersService UsersService = new UsersService();

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
                        var staffDataTask = UsersService.GetUsersWithRole(1);
                        var customersDataTask = UsersService.GetUsersWithRole(0);
                        customersDataTask.Wait();
                        staffDataTask.Wait();
                        CurrentUserRole = RoleById(details.Role.Id).Ok().OrElse(UserRole.Customer);
                        Customers = customersDataTask.Result.Ok().OrElse(new List<User>());
                        Staff = staffDataTask.Result.Ok().OrElse(new List<User>());
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