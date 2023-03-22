using System.Net;
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
    }
}