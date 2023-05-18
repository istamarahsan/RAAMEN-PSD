using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using PSD_Project.API.Features.Commerce.Orders;
using PSD_Project.App.Common;
using PSD_Project.App.Services.Orders;
using PSD_Project.App.Services.Users;
using Util.Collections;
using Util.Option;

namespace PSD_Project.App.Pages
{
    public partial class HandleOrders : Page
    {
        private readonly IOrderService orderService = AppServices.Singletons.OrderService;
        private readonly IUserService userService = AppServices.Singletons.UserService;

        protected List<OrderViewModel> Orders;

        protected void Page_Load(object sender, EventArgs e)
        {
            var getSession = Session.GetUserSession();
            var getToken = Request.GetTokenFromCookie();
            if (getToken.IsNone() || getSession.IsNone())
                Response.Redirect("Login.aspx");
            else if (getSession.Unwrap().Role.Name == "Customer") Response.Redirect("Home.aspx");


            var token = getToken.Unwrap();
            var session = Session.GetUserSession().Unwrap();
            InitPage(token);

            var getOrderIdToHandle = Request.QueryString["handle"]
                .ToOption()
                .Bind(s => s.TryParseInt().Ok());
            if (getOrderIdToHandle.IsSome())
            {
                var orderIdToHandle = getOrderIdToHandle.Unwrap();
                orderService.HandleOrder(token, orderIdToHandle)
                    .Unwrap($"Error handling order id: {orderIdToHandle}");
                Response.Redirect("Orders.aspx");
            }
        }

        protected void InitPage(int token)
        {
            var orders = orderService.GetOrders()
                .Unwrap("Failed to fetch orders");

            var customers = userService.GetCustomers(token)
                .Unwrap("Failed to fetch customers")
                .ToDictionary(c => c.Id);

            Orders = orders.Select(o => new OrderViewModel
                {
                    CustomerId = o.CustomerId,
                    CustomerUsername = customers.Get(o.CustomerId).Map(c => c.Username).OrElse("NOT FOUND"),
                    Id = o.Id,
                    Items = o.Items,
                    TimeCreated = o.TimeCreated
                })
                .ToList();
        }

        protected class OrderViewModel
        {
            public int CustomerId;
            public string CustomerUsername;
            public int Id;
            public List<CartItem> Items;
            public DateTime TimeCreated;
        }
    }
}