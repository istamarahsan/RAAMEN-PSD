using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Common
{
    public static class AdapterExtensions
    {
        public static Try<T, IHttpActionResult> InterpretAs<T>(
            this IHttpActionResult response)
        {
            return response.As<OkNegotiatedContentResult<T>>()
                .Map(result => result.Content)
                .OrErr(() => response);
        }

        public static T WithBearerToken<TController, T>(this TController controller, int token, Func<TController, T> operation) where TController : ApiController
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());
            controller.Request = request;
            var result = operation(controller);
            return result;
        }
    }
}