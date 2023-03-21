using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.Services;

namespace PSD_Project.API.Features.Commerce.Orders
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IAuthService authService;
        private readonly IOrdersHandler ordersHandler;

        public OrdersController()
        {
            ordersHandler = new OrdersHandler();
            authService = new AuthService();
        }

        public OrdersController(IOrdersHandler ordersHandler, IAuthService authService)
        {
            this.ordersHandler = ordersHandler;
            this.authService = authService;
        }

        [Route]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrders()
        {
            var orders = await ordersHandler.GetOrders();
            return orders.Match(Ok, HandleError);
        }
        
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            var order = await ordersHandler.GetOrder(id);
            return order.Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateOrder([FromBody] NewOrderDetails newOrderDetails)
        {
            var error = await ordersHandler.QueueOrder(newOrderDetails);
            return error.Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> HandleOrder(int id, [FromUri] int token)
        {
            var auth = await authService.GetSession(token);
            var handleOrder = await auth.Bind(userSession => ordersHandler.HandleOrder(id, userSession.Id));
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