using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using PSD_Project.API.Features.Ramen;
using PSD_Project.App.Common;
using Util.Option;
using Util.Try;
using IRamenService = PSD_Project.App.Services.RamenService.IRamenService;

namespace PSD_Project.App.Pages
{
    public partial class InsertRamen : Page
    {
        private readonly IRamenService ramenService = AppServices.Singletons.RamenService;
        private List<Meat> meats;
        private int token;

        protected void Page_Load(object sender, EventArgs e)
        {
            var getToken = Request.GetTokenFromCookie();
            var getSession = Session.GetUserSession();
            if (getToken.IsNone() || getSession.IsNone())
            {
                Response.Redirect("Login.aspx");
            }

            token = getToken.Unwrap();
            var session = getSession.Unwrap();
            if (session.Role.Name == "Customer")
            {
                Response.Redirect("Home.aspx");
            }
            
            InitPage(token);
        }

        private void InitPage(int token)
        {
            meats = ramenService.GetMeats().Unwrap("Failed to fetch meats");
            MeatDropDown.DataSource = meats.Select(m => m.Name);
            MeatDropDown.DataBind();
        }
        
        protected void SubmitButton_OnClick(object sender, EventArgs e)
        {
            var (ramenValidation, brothValidation, priceValidation) = (
                RamenNameTextBox.Text.Assert(s => s.Contains("Ramen"), _ => "Must contain Ramen").Err(),
                BrothTextBox.Text.Assert(s => s.Length != 0, _ => "Must not be empty").Err(),
                PriceTextBox.Text
                    .TryParseDouble()
                    .MapErr(_ => "Must be a number")
                    .Bind(p => p.Assert(x => x >= 3000.0, _ => "Must be at least 3000"))
                    .Err()
            );
            RamenNameErrorLabel.Text = ramenValidation.OrElse("");
            BrothErrorLabel.Text = brothValidation.OrElse("");
            PriceErrorLabel.Text = priceValidation.OrElse("");

            if (ramenValidation.IsNone() && brothValidation.IsNone() && priceValidation.IsNone())
            {
                ramenService.CreateRamen(token, new RamenDetails(
                    RamenNameTextBox.Text,
                    BrothTextBox.Text,
                    PriceTextBox.Text,
                    meats[MeatDropDown.SelectedIndex].Id
                )).Unwrap("Failed to create ramen");
                Response.Redirect("ManageRamen.aspx");
            }
        }
    }
}