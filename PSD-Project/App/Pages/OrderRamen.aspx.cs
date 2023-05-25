using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.API.Features.Ramen;
using PSD_Project.App.Common;
using PSD_Project.App.Services.Orders;
using Util.Collections;
using Util.Option;
using IRamenService = PSD_Project.App.Services.RamenService.IRamenService;

namespace PSD_Project.App.Pages
{
    public partial class OrderRamen : Page
    {
        private readonly IRamenService ramenService = AppServices.Singletons.RamenService;
        private readonly IOrderService orderService = AppServices.Singletons.OrderService;

        protected Dictionary<int, Ramen> Ramen;
        protected Dictionary<int, int> Cart;

        protected void Page_Load(object sender, EventArgs e)
        {
            var loggedInAsCustomer = Session.GetUserSession()
                .Match(
                    session => session.Role.Name == "Customer",
                    () => false);
            if (!loggedInAsCustomer) Response.Redirect("Home.aspx");
            
            if (Session["Cart"] == null) Session["Cart"] = new Dictionary<int, int>();
            Cart = Session["Cart"] as Dictionary<int, int>;
            var ramenIdOption = Request.QueryString["ramenId"].ToOption().Bind(x => x.TryParseInt().Ok());
            var quantityOption = Request.QueryString["quantity"].ToOption().Bind(x => x.TryParseInt().Ok());
            if (ramenIdOption.IsSome() && quantityOption.IsSome())
            {
                var ramenId = ramenIdOption.Unwrap();
                var quantity = quantityOption.Unwrap();
                Cart[ramenId] = Math.Max(Cart.Get(ramenId).OrElse(0) + quantity, 0);
                Session["Cart"] = Cart;
            }
            
            if (IsPostBack) return;
            Ramen = ramenService.GetRamen()
                .Map(ramen => ramen.ToDictionary(r => r.Id))
                .Unwrap();
        }


        protected void ClearCartButton_OnClick(object sender, EventArgs e)
        {
            Cart = null;
            Session["Cart"] = null;
        }

        protected void PlaceOrderButton_OnClick(object sender, EventArgs e)
        {
            var userId = Session.GetUserSession().Unwrap().Id;
            var items = (Session["Cart"] as Dictionary<int, int> ?? new Dictionary<int, int>())
                .Select(pair => new CartItem(pair.Key, pair.Value))
                .ToList();
            if (!(items.Count > 0)) return;
            orderService.PlaceOrder(Request.GetTokenFromCookie().Unwrap(), new NewOrderDetails(userId, items));
            Cart = null;
            Session["Cart"] = null;
        }
    }
}