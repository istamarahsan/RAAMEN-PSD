using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Common;
using PSD_Project.App.Services.Users;

namespace PSD_Project.App.Pages
{
    public partial class Profile : Page
    {
        private readonly IUserService usersService = AppServices.Singletons.UserService;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.GetUserSession().Match(
                some: _ => { },
                none: () =>
                {
                    Response.Redirect("Login.aspx");
                });
        }

        protected void UpdateButton_OnClick(object sender, EventArgs e)
        {
            var token = Request.GetTokenFromCookie().Unwrap();
            var userId = Session.GetUserSession().Unwrap().Id;
            // TODO: Validate new details
            usersService.UpdateUserDetails(token, userId, new UserUpdateDetails(
                username: UsernameTextBox.Text,
                email: EmailTextBox.Text,
                gender: GenderDropDownList.SelectedValue.Trim(' ', '\n'))
            );
            Response.Redirect("Home.aspx");
        }
    }
}