using System;
using Util.Option;

namespace Util.Try
{
    public sealed class TryErr<T, TErr> : Try<T, TErr>
    {
        private readonly TErr _data;

        internal TryErr(TErr data)
        {
            _data = data;
        }

        public override void Match(Action<T> ok, Action<TErr> err)
        {
            err(_data);
        }

        public override TOut Match<TOut>(Func<T, TOut> ok, Func<TErr, TOut> err)
        {
            return err(_data);
        }

        public override Try<TOut, TErr> Map<TOut>(Func<T, TOut> f)
        {
            return new TryErr<TOut, TErr>(_data);
        }

        public override Try<TOut, TErr> Cast<TOut>(Func<TErr> @catch)
        {
            return new TryErr<TOut, TErr>(_data);
        }

        public override Try<TOut, TErr> Bind<TOut>(Func<T, Try<TOut, TErr>> f)
        {
            return new TryErr<TOut, TErr>(_data);
        }

        public override Option<T> Ok()
        {
            return Option.Option.None<T>();
        }

        public override Option<TErr> Err()
        {
            return Option.Option.Some(_data);
        }

        public override bool IsOk()
        {
            return false;
        }

        public override bool IsErr()
        {
            return true;
        }
    }
}