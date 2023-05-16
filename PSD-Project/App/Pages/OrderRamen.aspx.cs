using System;
using System.Collections.Generic;
using System.Web.UI;
using PSD_Project.API.Features.Ramen;
using PSD_Project.App.Common;
using IRamenService = PSD_Project.App.Services.RamenService.IRamenService;

namespace PSD_Project.App.Pages
{
    public partial class OrderRamen : Page
    {
        private readonly IRamenService ramenService = AppServices.Singletons.RamenService;

        protected List<Ramen> Ramen = new List<Ramen>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            var loggedInAsCustomer = Session.GetUserSession()
                .Match(
                    session => session.Role.Name == "Customer",
                    () => false);
            if (!loggedInAsCustomer) Response.Redirect("Home.aspx");
            Ramen = ramenService.GetRamen()
                .Unwrap();
        }
        
        
    }
}