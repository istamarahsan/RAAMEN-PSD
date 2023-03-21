using System;
using System.Runtime;
using System.Threading.Tasks;
using Util.Option;

namespace Util.Try
{
    public abstract class Try<T, TErr>
    {
        public abstract void Match(Action<T> ok, Action<TErr> err);
        public abstract TOut Match<TOut>(Func<T, TOut> ok, Func<TErr, TOut> err);
        public abstract Try<TOut, TErr> Map<TOut>(Func<T, TOut> f);
        public abstract Try<T, TOut> MapErr<TOut>(Func<TErr, TOut> f);
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

        public static Try<T, TErr> Assert<T, TErr>(this T @object, Func<T, bool> assertion, Func<T, TErr> otherwise)
        {
            return assertion(@object) 
                ? new TryOk<T, TErr>(@object) as Try<T, TErr>
                : new TryErr<T, TErr>(otherwise(@object));
        }

        public static Try<T, TErr> Assert<T, TErr>(this bool statement, bool expected, Func<T> @true, Func<TErr> @false)
        {
            return statement == expected
                ? new TryOk<T, TErr>(@true()) as Try<T, TErr>
                : new TryErr<T, TErr>(@false());
        }
 
        public static Try<T, TErr> OrErr<T, TErr>(this Option<T> option, Func<TErr> otherwise)
        {
            return option.Match(
                some: Of<T, TErr>,
                none: () => Err<T, TErr>(otherwise()));
        }

        public static Func<TIn, Try<TOut, Exception>> OfFallible<TIn, TOut>(Func<TIn, TOut> fallible)
        {
            return @in =>
            {
                try
                {
                    return Of<TOut, Exception>(fallible(@in));
                }
                catch (Exception e)
                {
                    return Err<TOut, Exception>(e);
                }
            };
        }
    }
}