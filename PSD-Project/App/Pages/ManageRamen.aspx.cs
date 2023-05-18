using System;
using System.Collections.Generic;
using System.Web.UI;
using PSD_Project.API.Features.Ramen;
using PSD_Project.App.Common;
using IRamenService = PSD_Project.App.Services.RamenService.IRamenService;

namespace PSD_Project.App.Pages
{
    public partial class ManageRamen : Page
    {
        protected List<Ramen> Ramen;
        private readonly IRamenService ramenService = AppServices.Singletons.RamenService;
        protected void Page_Load(object sender, EventArgs e)
        {
            var getToken = Request.GetTokenFromCookie();
            var getSession = Session.GetUserSession();
            if (getToken.IsNone() || getSession.IsNone())
            {
                Response.Redirect("Login.aspx");
            }

            var token = getToken.Unwrap();
            var session = getSession.Unwrap();
            if (session.Role.Name == "Customer")
            {
                Response.Redirect("Home.aspx");
            }
            InitPage(token);
        }

        private void InitPage(int token)
        {
            Ramen = ramenService.GetRamen().Unwrap("Failed to fetch ramen");
        }
    }
}