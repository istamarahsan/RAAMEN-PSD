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

        public override async Task<TOut> Match<TOut>(Func<T, Task<TOut>> ok, Func<TErr, Task<TOut>> err)
        {
            return await err(Value);
        }

        public override Try<TOut, TErr> Map<TOut>(Func<T, TOut> f)
        {
            return new TryErr<TOut, TErr>(Value);
        }

        public override Task<Try<TOut, TErr>> Map<TOut>(Func<T, Task<TOut>> f)
        {
            return Task.FromResult(new TryErr<TOut, TErr>(Value) as Try<TOut, TErr>);
        }

        public override Try<T, TOut> MapErr<TOut>(Func<TErr, TOut> f)
        {
            return new TryErr<T, TOut>(f(Value));
        }

        public override async Task<Try<T, TOut>> MapErr<TOut>(Func<TErr, Task<TOut>> f)
        {
            return new TryErr<T, TOut>(await f(Value));
        }


        public override Try<TOut, TErr> Cast<TOut>(Func<TErr> @catch)
        {
            return new TryErr<TOut, TErr>(Value);
        }

        public override Try<TOut, TErr> Bind<TOut>(Func<T, Try<TOut, TErr>> f)
        {
            return new TryErr<TOut, TErr>(Value);
        }

        public override Task<Try<TOut, TErr>> Bind<TOut>(Func<T, Task<Try<TOut, TErr>>> f)
        {
            return Task.FromResult(new TryErr<TOut, TErr>(Value) as Try<TOut, TErr>);
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
    }
}