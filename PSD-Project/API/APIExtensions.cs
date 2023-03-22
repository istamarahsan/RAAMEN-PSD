using System;
using System.Net.Http;
using PSD_Project.App.Common;
using Util.Option;
using Util.Try;

namespace PSD_Project.API
{
    public static class ApiExtensions
    {
        public static Try<int, Exception> ExtractAuthToken(this HttpRequestMessage message)
        {
            return message.Headers
                .Authorization
                .ToOption()
                .Bind(authVal => authVal.Parameter.ToOption())
                .Bind(param => param.TryParseInt().Ok())
                .OrErr<int, Exception>(() => new UnauthorizedAccessException("Unauthorized"));
        }
    }
}