using System;
using System.Threading.Tasks;
using Util.Option;

namespace Util.Try
{
    public sealed class TryOk<T, TErr> : Try<T, TErr>
    {
        public readonly T Value;

        internal TryOk(T value)
        {
            Value = value;
        }

        public override void Match(Action<T> ok, Action<TErr> err)
        {
            ok(Value);
        }

        public override TOut Match<TOut>(Func<T, TOut> ok, Func<TErr, TOut> err)
        {
            return ok(Value);
        }
        
        public override Try<TOut, TErr> Map<TOut>(Func<T, TOut> f)
        {
            return new TryOk<TOut, TErr>(f(Value));
        }

        public override Try<T, TOut> MapErr<TOut>(Func<TErr, TOut> f)
        {
            return new TryOk<T, TOut>(Value);
        }

        public override T Recover(Func<TErr, T> f)
        {
            return Value;
        }

        public override Try<TOut, TErr> Cast<TOut>(Func<TErr> @catch)
        {
            return Value is TOut @out 
                ? new TryOk<TOut, TErr>(@out) as Try<TOut, TErr>
                : new TryErr<TOut, TErr>(@catch());
        }

        public override Try<TOut, TErr> Bind<TOut>(Func<T, Try<TOut, TErr>> f)
        {
            return f(Value);
        }

        public override Option<T> Ok()
        {
            return Option.Option.Some(Value);
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

        public override T Unwrap() => Value;
        public override T Unwrap(string expect) => Value;

        public override TErr UnwrapError() => throw new InvalidOperationException("No Error to Unwrap");
    }
}