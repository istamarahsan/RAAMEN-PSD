using System;
using System.Threading.Tasks;
using Util.Option;

namespace Util.Try
{
    public sealed class TryErr<T, TErr> : Try<T, TErr>
    {
        public readonly TErr Value;

        internal TryErr(TErr value)
        {
            Value = value;
        }

        public override void Match(Action<T> ok, Action<TErr> err)
        {
            err(Value);
        }

        public override TOut Match<TOut>(Func<T, TOut> ok, Func<TErr, TOut> err)
        {
            return err(Value);
        }
        
        public override Try<TOut, TErr> Map<TOut>(Func<T, TOut> f)
        {
            return new TryErr<TOut, TErr>(Value);
        }

        public override Try<T, TOut> MapErr<TOut>(Func<TErr, TOut> f)
        {
            return new TryErr<T, TOut>(f(Value));
        }

        public override T Recover(Func<TErr, T> f)
        {
            return f(Value);
        }

        public override Try<TOut, TErr> Cast<TOut>(Func<TErr> @catch)
        {
            return new TryErr<TOut, TErr>(Value);
        }

        public override Try<TOut, TErr> Bind<TOut>(Func<T, Try<TOut, TErr>> f)
        {
            return new TryErr<TOut, TErr>(Value);
        }
        
        public override Option<T> Ok()
        {
            return Option.Option.None<T>();
        }

        public override Option<TErr> Err()
        {
            return Option.Option.Some(Value);
        }

        public override bool IsOk()
        {
            return false;
        }

        public override bool IsErr()
        {
            return true;
        }

        public override T Unwrap() => throw (Value as Exception ?? new InvalidOperationException("No value to unwrap."));
        
        public override T Unwrap(string expect) => throw (Value is Exception exception
            ? new InvalidOperationException(expect, exception)
            : new InvalidOperationException(expect));

        public override TErr UnwrapError() => Value;
    }
}