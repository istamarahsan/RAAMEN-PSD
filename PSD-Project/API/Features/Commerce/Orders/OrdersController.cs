using System;
using System.Web.Http;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users.Authorization;
using PSD_Project.API.Util.ApiController;
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

        public OrdersController(IOrdersService ordersService, IAuthorizationService authorizationService,
            IAuthenticationService authenticationService)
        {
            this.ordersService = ordersService;
            this.authorizationService = authorizationService;
            this.authenticationService = authenticationService;
        }

        [Route]
        [HttpGet]
        public IHttpActionResult GetOrders() => ordersService.GetOrders().Match(Ok, HandleError);

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetOrder(int id) => ordersService.GetOrder(id).Match(Ok, HandleError);

        [Route]
        [HttpPost]
        public IHttpActionResult PlaceOrder([FromBody] NewOrderDetails newOrderDetails)
        {
            if (newOrderDetails == null) return BadRequest();
            return Request.ExtractAuthToken()
                .Bind(authenticationService.GetSession)
                .Bind(user => user.Id == newOrderDetails.CustomerId
                    ? Try.Of<int, Exception>(user.Role.Id)
                    : Try.Err<int, Exception>(new UnauthorizedAccessException()))
                .Bind(roleId => authorizationService.RoleHasPermission(roleId, Permission.PlaceOrder)
                    ? ordersService.QueueOrder(newOrderDetails)
                    : Try.Err<Order, Exception>(new UnauthorizedAccessException()))
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
                case UnauthorizedAccessException _:
                    return Unauthorized();
                default:
                    return InternalServerError();
            }
        }
    }
}