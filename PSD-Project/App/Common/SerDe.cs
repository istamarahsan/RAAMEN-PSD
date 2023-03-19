using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Common
{
    public static class SerDe
    {
        
        
        public static Try<HttpContent, Exception> TryGetContent(this HttpResponseMessage response)
        {
            return response.Check(
                    r => r.StatusCode == HttpStatusCode.OK,
                    r => new HttpException(r.StatusCode.ToString()))
                .Map(r => r.Content)
                .MapErr(e => e as Exception);
        }
        
        public static Try<string, Exception> TryReadResponseString(this HttpContent content)
        {
            if (content == null) return Try.Err<string, Exception>(new NullReferenceException());
            var stringTask = content.ReadAsStringAsync();
            stringTask.Wait();
            return stringTask.Status == TaskStatus.RanToCompletion 
                ? Try.Of<string, Exception>(stringTask.Result) 
                : Try.Err<string, Exception>(stringTask.Exception);
        }
        
        public static Try<T, Exception> TryDeserializeJson<T>(this string jsonString) where T : class
        {
            return jsonString.ToOption()
                .OrErr(() => new NullReferenceException())
                .MapErr(err => err as Exception)
                .Bind(str => Try.Of<object, Exception, Exception>(() => JsonConvert.DeserializeObject(str, typeof(T)), e => e))
                .Cast<T>(() => new Exception("Deserialization result was not of expected type"));
        }
    }
}