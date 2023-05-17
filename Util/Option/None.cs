using System;

namespace Util.Option
{
    public class None<T> : Option<T>
    {
        internal None() {}
        public override void Match(Action<T> some, Action none) => none();
        public override TOut Match<TOut>(Func<T, TOut> some, Func<TOut> none) => none();

        public override Option<TOut> Map<TOut>(Func<T, TOut> mapper) => new None<TOut>();
        public override Option<TOut> Bind<TOut>(Func<T, Option<TOut>> func) => new None<TOut>();

        public override Option<TOut> Cast<TOut>() => new None<TOut>();
        public override T OrElse(T value) => value;
        public override bool IsSome() => false;

        public override bool IsNone() => true;
        public override T Unwrap() => throw new InvalidOperationException();
        public override T Unwrap(string expect) => throw new InvalidOperationException(expect);
    }
}