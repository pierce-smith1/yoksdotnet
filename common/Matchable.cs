using System;

namespace yoksdotnet.common;

public class Matchable<A, B>
{
    protected A? VariantA { get; init; }
    protected B? VariantB { get; init; }

    public T Match<T>(Func<A, T> whenA, Func<B, T> whenB)
    {
        if (VariantA is not null) return whenA(VariantA);
        if (VariantB is not null) return whenB(VariantB);

        throw new InvalidOperationException();
    }

    public void Switch<T>(Action<A> whenA, Action<B> whenB)
    {
        if (VariantA is not null) whenA(VariantA);
        if (VariantB is not null) whenB(VariantB);

        throw new InvalidOperationException();
    }
}
