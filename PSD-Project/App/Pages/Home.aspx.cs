using System;
using System.Web.UI;
using PSD_Project.API.Features.Authentication;
using PSD_Project.App.Common;

namespace PSD_Project.App.Pages
{
    public partial class Home : Page
    {
        protected UserSessionDetails UserSession;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.GetUserSession().Match(
                some: session => UserSession = session,
                none: () => Response.Redirect("Login.aspx"));
        }
        
        
    }
}