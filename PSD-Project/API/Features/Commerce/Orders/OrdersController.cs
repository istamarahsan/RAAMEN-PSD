using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Service;

namespace PSD_Project.API.Features.Commerce.Orders
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IOrdersService ordersService;

        public OrdersController()
        {
            ordersService = Services.GetOrdersService();
            authenticationService = Services.GetAuthenticationService();
        }

        public OrdersController(IOrdersService ordersService, IAuthenticationService authenticationService)
        {
            this.ordersService = ordersService;
            this.authenticationService = authenticationService;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetOrders()
        {
            var orders = ordersService.GetOrders();
            return orders.Match(Ok, HandleError);
        }
        
        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetOrder(int id)
        {
            var order = ordersService.GetOrder(id);
            return order.Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult CreateOrder([FromBody] NewOrderDetails newOrderDetails)
        {
            var error = ordersService.QueueOrder(newOrderDetails);
            return error.Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpPost]
        public IHttpActionResult HandleOrder(int id, [FromUri] int token)
        {
            return authenticationService.GetSession(token)
                .Bind(userSession => ordersService.HandleOrder(id, userSession.Id))
                .Match(Ok, HandleError);
        }
        
        private IHttpActionResult HandleError(Exception e)
        {
            switch (e)
            {
                default:
                    return InternalServerError();
            }
        }
    }
}