using System;

namespace Util.Option
{
    public class Some<T> : Option<T>
    {
        private readonly T data;

        internal Some(T data)
        {
            this.data = data;
        }

        public override void Match(Action<T> some, Action none) => some(data);
        public override TOut Match<TOut>(Func<T, TOut> some, Func<TOut> none) => some(data);

        public override Option<TOut> Map<TOut>(Func<T, TOut> mapper) => new Some<TOut>(mapper(data));
        public override Option<TOut> Bind<TOut>(Func<T, Option<TOut>> func) => func(data);

        public override Option<TOut> Cast<TOut>() => data is TOut d ? new Some<TOut>(d) : Option.None<TOut>();
        public override T OrElse(T value) => data;
    }
}