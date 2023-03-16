using System;
using System.Runtime;
using Util.Option;

namespace Util.Try
{
    public abstract class Try<T, TErr>
    {
        public abstract void Match(Action<T> ok, Action<TErr> err);
        public abstract TOut Match<TOut>(Func<T, TOut> ok, Func<TErr, TOut> err);
        public abstract Try<TOut, TErr> Map<TOut>(Func<T, TOut> f);
        public abstract Try<TOut, TErr> Cast<TOut>(Func<TErr> @catch);
        public abstract Try<TOut, TErr> Bind<TOut>(Func<T, Try<TOut, TErr>> f);
        public abstract Option<T> Ok();
        public abstract Option<TErr> Err();
        public abstract bool IsOk();
        public abstract bool IsErr();
    }

    public static class Try
    {
        public static Try<T, TErr> Of<T, TErr>(T data)
        {
            return new TryOk<T, TErr>(data);
        }
        
        public static Try<T, TErr> Err<T, TErr>(TErr err)
        {
            return new TryErr<T, TErr>(err);
        }

        public static Try<T, TErr> Of<T, TErr, TException>(Func<T> fallible, Func<TException, TErr> recover) where TException : Exception
        {
            try
            {
                return new TryOk<T, TErr>(fallible());
            }
            catch (TException e)
            {
                return new TryErr<T, TErr>(recover(e));
            }
        }

        public static Try<T, TErr> Check<T, TErr>(this T input, Func<T, bool> check, Func<T, TErr> otherwise)
        {
            return check(input) 
                ? new TryOk<T, TErr>(input) as Try<T, TErr>
                : new TryErr<T, TErr>(otherwise(input));
        }
    }
}