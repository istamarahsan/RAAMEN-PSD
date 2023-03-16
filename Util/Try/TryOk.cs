using System;
using Util.Option;

namespace Util.Try
{
    public sealed class TryOk<T, TErr> : Try<T, TErr>
    {
        private readonly T _data;

        internal TryOk(T data)
        {
            _data = data;
        }

        public override void Match(Action<T> ok, Action<TErr> err)
        {
            ok(_data);
        }

        public override TOut Match<TOut>(Func<T, TOut> ok, Func<TErr, TOut> err)
        {
            return ok(_data);
        }

        public override Try<TOut, TErr> Map<TOut>(Func<T, TOut> f)
        {
            return new TryOk<TOut, TErr>(f(_data));
        }

        public override Try<TOut, TErr> Cast<TOut>(Func<TErr> @catch)
        {
            return _data is TOut @out 
                ? new TryOk<TOut, TErr>(@out) as Try<TOut, TErr>
                : new TryErr<TOut, TErr>(@catch());
        }

        public override Try<TOut, TErr> Bind<TOut>(Func<T, Try<TOut, TErr>> f)
        {
            return f(_data);
        }

        public override Option<T> Ok()
        {
            return Option.Option.Some(_data);
        }

        public override Option<TErr> Err()
        {
            return Option.Option.None<TErr>();
        }

        public override bool IsOk()
        {
            return true;
        }

        public override bool IsErr()
        {
            return false;
        }
    }
}