using System;
using System.Collections.Generic;
using System.Web.UI;
using PSD_Project.App.Common;
using PSD_Project.App.Models;
using PSD_Project.Features.Users;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Pages
{
    public partial class Home : Page
    {
        private static readonly IAuthService AuthService = new LoginAuthService();
        private static readonly IUsersService UsersService = new UsersService();

        protected UserRole CurrentUserRole = UserRole.Customer;
        protected List<User> Customers = new List<User>();
        protected List<User> Staff = new List<User>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Request.Cookies[Globals.SessionCookieName]
                .ToOption()
                .Map(cookie => cookie.Value)
                .Bind(val => Try.Of<int, Exception, Exception>(() => int.Parse(val), exc => exc).Ok())
                .Bind(token =>
                {
                    var authTask = AuthService.Authenticate(token);
                    authTask.Wait();
                    return authTask.Result.Ok();
                })
                .Match(
                    some: details =>
                    {
                        RoleLabel.Text = details.Role.Name;
                        var staffDataTask = UsersService.TryGetUsersWithRoleAsync(1);
                        var customersDataTask = UsersService.TryGetUsersWithRoleAsync(0);
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