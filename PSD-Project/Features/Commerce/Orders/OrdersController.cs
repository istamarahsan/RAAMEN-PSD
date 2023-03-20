using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.Features.Commerce.Transactions;

namespace PSD_Project.Features.Commerce.Orders
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrdersHandler ordersHandler;

        public OrdersController()
        {
            ordersHandler = new CommerceHandler();
        }

        public OrdersController(IOrdersHandler ordersHandler)
        {
            this.ordersHandler = ordersHandler;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CreateOrder([FromBody] Order order)
        {
            IHttpActionResult HandleQueueOrderException(Exception e)
            {
                switch (e)
                {
                    default:
                        return InternalServerError();
                }
            }
            
            var error = await ordersHandler.QueueOrderAsync(order);
            return error.Match(Ok, HandleQueueOrderException);
        }
    }
}