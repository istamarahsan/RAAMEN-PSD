using System;
using System.Web;
using System.Web.UI;
using PSD_Project.App.Common;

namespace PSD_Project.App.Pages
{
    public partial class Main : MasterPage
    {
        protected enum LoggedInAsRole
        {
            Customer,
            Staff,
            Admin
        }

        protected LoggedInAsRole LoggedInAs = LoggedInAsRole.Customer;

        protected void Page_Load(object sender, EventArgs e)
        {
            var getSession = Session.GetUserSession();
            if (getSession.IsNone())
            {
                Response.Redirect("Login.aspx");
                return;
            }

            switch (getSession.Unwrap().Role.Name)
            {
                case "Admin":
                    LoggedInAs = LoggedInAsRole.Admin;
                    break;
                case "Staff":
                    LoggedInAs = LoggedInAsRole.Staff;
                    break;
                default:
                    LoggedInAs = LoggedInAsRole.Customer;
                    break;
            }
        }

        protected void LogoutButton_OnClick(object sender, EventArgs e)
        {
            Session[Globals.SavedSessionName] = null;
            Response.SetCookie(new HttpCookie(Globals.SessionCookieName, ""));
            Response.Redirect("Login.aspx");
        }
    }
}