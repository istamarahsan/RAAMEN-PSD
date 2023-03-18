using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Util.Option;
using Util.Try;

namespace PSD_Project.App.Common
{
    public static class SerDe
    {
        public static Option<string> TryReadResponseString(this HttpContent content)
        {
            if (content == null) return Option.None<string>();
            var stringTask = content.ReadAsStringAsync();
            while (stringTask.Status == TaskStatus.Running) { }
            return stringTask.Status == TaskStatus.RanToCompletion 
                ? Option.Some(stringTask.Result) 
                : Option.None<string>();
        }
        
        public static Option<T> TryDeserializeJson<T>(this string jsonString) where T : class
        {
            return jsonString.ToOption()
                .Bind(str => Try.OfFallible<string, object>(s => JsonConvert.DeserializeObject(s, typeof(T)))(str).Ok())
                .Cast<T>();
        }
    }
}