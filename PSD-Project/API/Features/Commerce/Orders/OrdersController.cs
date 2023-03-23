using System;
using System.Threading.Tasks;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util;
using PSD_Project.API.Util.ApiController;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Commerce.Orders
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IAuthorizationService authorizationService;
        private readonly IOrdersService ordersService;

        public OrdersController()
        {
            ordersService = Services.GetOrdersService();
            authorizationService = Services.GetAuthorizationService();
            authenticationService = Services.GetAuthenticationService();
        }

        public OrdersController(IOrdersService ordersService, IAuthorizationService authorizationService, IAuthenticationService authenticationService)
        {
            this.ordersService = ordersService;
            this.authorizationService = authorizationService;
            this.authenticationService = authenticationService;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetOrders()
        {
            return ordersService.GetOrders().Match(Ok, HandleError);
        }
        
        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetOrder(int id)
        {
            return ordersService.GetOrder(id).Match(Ok, HandleError);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult PlaceOrder([FromBody] NewOrderDetails newOrderDetails)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Map(user => user.Role.Id)
                .Map(roleId => authorizationService.RoleHasPermission(roleId, Permission.PlaceOrder))
                .Bind(hasPermission => hasPermission
                    ? newOrderDetails.ToOption().OrErr<NewOrderDetails, Exception>(() => new ArgumentException())
                    : Try.Err<NewOrderDetails, Exception>(new UnauthorizedAccessException()))
                .Bind(ordersService.QueueOrder)
                .Match(Ok, HandleError);
        }

        [Route("{id}")]
        [HttpPost]
        public IHttpActionResult HandleOrder(int id)
        {
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Bind(userSession => ordersService.HandleOrder(id, userSession.Id))
                .Match(Ok, HandleError);
        }
        
        private IHttpActionResult HandleError(Exception e)
        {
            switch (e)
            {
                case ArgumentException _:
                    return BadRequest();
                default:
                    return InternalServerError();
            }
        }
    }
}