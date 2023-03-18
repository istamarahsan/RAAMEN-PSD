using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.UI;
using PSD_Project.App.Common;
using PSD_Project.Features.LogIn;
using PSD_Project.Features.Users;
using Util.Option;

namespace PSD_Project.App.Pages
{
    public partial class Home : Page
    {
        private static readonly Uri LoginServiceUri = new Uri("http://localhost:5000/api/login");
        private static readonly Uri UsersServiceUri = new Uri("http://localhost:5000/api/users");

        protected enum UserRole
        {
            Default,
            Staff,
            Admin
        }

        protected UserRole CurrentUserRole = UserRole.Default;
        protected List<User> Customers = new List<User>();
        protected List<User> Staff = new List<User>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Request.Cookies["raamen-session"]
                .ToOption()
                .Map(cookie => cookie.Value)
                .Match(
                    token =>
                    {
                        var sessionTask =
                            RaamenApp.HttpClient.GetAsync(new Uri(LoginServiceUri, $"?SessionToken={token}"));
                        sessionTask.Wait();
                        sessionTask.Check(task => task.Status == TaskStatus.RanToCompletion)
                            .Map(task => task.Result)
                            .Bind(result => result.Check(r => r.StatusCode == HttpStatusCode.OK))
                            .Map(s => s.Content)
                            .Bind(content => content.TryReadResponseString())
                            .Bind(str => str.TryDeserializeJson<UserSessionDetails>())
                            .Match(
                                details =>
                                {
                                    RoleLabel.Text = details.Role.Name;
                                    CurrentUserRole = details.Role.Id == 1
                                        ? UserRole.Staff
                                        : details.Role.Id == 2
                                            ? UserRole.Admin
                                            : UserRole.Default;
                                    var getStaffDataTask =
                                        RaamenApp.HttpClient.GetAsync(new Uri(UsersServiceUri, "?roleId=1"));
                                    var getCustomersDataTask =
                                        RaamenApp.HttpClient.GetAsync(new Uri(UsersServiceUri, "?roleId=0"));
                                    while (getStaffDataTask.Status == TaskStatus.Running || getCustomersDataTask.Status == TaskStatus.Running)
                                    {
                                        
                                    }

                                    Customers = getCustomersDataTask.Result.Content
                                        .TryReadResponseString()
                                        .Bind(str => str.TryDeserializeJson<List<User>>())
                                        .OrElse(new List<User>());

                                    Staff = getStaffDataTask.Result.Content
                                        .TryReadResponseString()
                                        .Bind(str => str.TryDeserializeJson<List<User>>())
                                        .OrElse(new List<User>());
                                },
                                () => { Response.Redirect("Login.aspx"); });
                    },
                    () => { Response.Redirect("Login.aspx"); });
        }
    }
}