using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.Features.Commerce.Transactions;
using PSD_Project.Services;

namespace PSD_Project.Features.Commerce.Orders
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrdersHandler ordersHandler;
        private readonly IAuthService authService;

        public OrdersController()
        {
            ordersHandler = new CommerceHandler();
            authService = new LoginAuthService();
        }

        public OrdersController(IOrdersHandler ordersHandler, IAuthService authService)
        {
            this.ordersHandler = ordersHandler;
            this.authService = authService;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateOrder([FromBody] NewOrderDetails newOrderDetails)
        {
            IHttpActionResult HandleQueueOrderException(Exception e)
            {
                switch (e)
                {
                    default:
                        return InternalServerError();
                }
            }
            
            var error = await ordersHandler.QueueOrderAsync(newOrderDetails);
            return error.Match(Ok, HandleQueueOrderException);
        }

        
        [Route("{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> HandleOrder(int id, [FromUri] int token)
        {
            IHttpActionResult HandleError(Exception e)
            {
                switch (e)
                {
                    case ArgumentException _ :
                        return NotFound();
                    default:
                        return InternalServerError();
                }
            }

            var auth = await authService.Authenticate(token);
            var handleOrder = await auth.Bind(userSession => ordersHandler.HandleOrderAsync(id, userSession.Id));
            return handleOrder.Match(Ok, HandleError);
        }
    }
}