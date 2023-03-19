using System;

namespace Util.Option
{
    public abstract class Option<T>
    {
        public abstract void Match(Action<T> some, Action none);
        public abstract TOut Match<TOut>(Func<T, TOut> some, Func<TOut> none);
        public abstract Option<TOut> Map<TOut>(Func<T, TOut> mapper);
        public abstract Option<TOut> Bind<TOut>(Func<T, Option<TOut>> func);
        public abstract Option<TOut> Cast<TOut>() where TOut : class;
        public abstract T OrElse(T value);
        public abstract bool IsSome();
        public abstract bool IsNone();
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value) => new Some<T>(value);
        public static Option<T> None<T>() => new None<T>();

        public static Option<T> ToOption<T>(this T nullableObject) =>
            nullableObject != null 
                ? Some(nullableObject) 
                : None<T>();

        public static Option<T> Check<T>(this T input, Predicate<T> check)
        {
            return check(input)
                ? Some(input)
                : None<T>();
        }
    }
}