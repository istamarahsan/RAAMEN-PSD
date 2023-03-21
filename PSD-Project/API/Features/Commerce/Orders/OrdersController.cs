using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.Service;

namespace PSD_Project.API.Features.Commerce.Orders
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IAuthService authService;
        private readonly IOrdersService ordersService;

        public OrdersController()
        {
            ordersService = Services.GetOrdersService();
            authService = Services.GetAuthService();
        }

        public OrdersController(IOrdersService ordersService, IAuthService authService)
        {
            this.ordersService = ordersService;
            this.authService = authService;
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrders()
        {
            var orders = await ordersService.GetOrders();
            return orders.Match(Ok, HandleError);
        }
        
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            var order = await ordersService.GetOrder(id);
            return order.Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateOrder([FromBody] NewOrderDetails newOrderDetails)
        {
            var error = await ordersService.QueueOrder(newOrderDetails);
            return error.Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> HandleOrder(int id, [FromUri] int token)
        {
            var auth = await authService.GetSession(token);
            var handleOrder = await auth.Bind(userSession => ordersService.HandleOrder(id, userSession.Id));
            return handleOrder.Match(Ok, HandleError);
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