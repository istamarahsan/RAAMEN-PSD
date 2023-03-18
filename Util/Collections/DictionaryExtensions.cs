using System.Collections.Generic;
using Util.Option;

namespace Util.Collections
{
    public static class DictionaryExtensions
    {
        public static Option<TValue> Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ContainsKey(key) 
                ? Option.Option.Some(dict[key]) 
                : Option.Option.None<TValue>();
        }
    }
}