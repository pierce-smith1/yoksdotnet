using System;

namespace yoksdotnet.common;

public abstract record Union<A, B>
{
    public R Match<R>(Func<A, R> whenA, Func<B, R> whenB)
    {
        if (this is A a) return whenA(a);
        if (this is B b) return whenB(b);

        throw new NotSupportedException();
    }
}

public abstract record Union<A, B, C>
{
    public R Match<R>(Func<A, R> whenA, Func<B, R> whenB, Func<C, R> whenC)
    {
        if (this is A a) return whenA(a);
        if (this is B b) return whenB(b);
        if (this is C c) return whenC(c);

        throw new NotSupportedException();
    }
}

public abstract record Union<A, B, C, D>
{
    public R Match<R>(Func<A, R> whenA, Func<B, R> whenB, Func<C, R> whenC, Func<D, R> whenD)
    {
        if (this is A a) return whenA(a);
        if (this is B b) return whenB(b);
        if (this is C c) return whenC(c);
        if (this is D d) return whenD(d);

        throw new NotSupportedException();
    }
}
