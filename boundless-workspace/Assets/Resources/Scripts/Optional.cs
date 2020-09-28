using System.Collections.Generic;
using System;

public static class Optional
{
    public static Optional<T> Of<T>(T value)
    {
        if (value == null)
        {
            throw new NullReferenceException();
        }
        return new Optional<T>(present: true, value: value);
    }

    public static Optional<T> Empty<T>()
    {
        return new Optional<T>(present: false);
    }

    public static Optional<T> OfNullable<T>(T valueOrNull) where T : class
    {
        return new Optional<T>(present: valueOrNull != null, value: valueOrNull);
    }
}

public class Optional<T>
{
    private readonly bool present;
    private readonly T value;

    // Don't use this constructor from outside this file. I'm not sure if there's an access
    // modifier that would let me enforce this.
    internal Optional(bool present, T value = default(T))
    {
        this.present = present;
        this.value = value;
    }

	public Optional<U> Map<U>(Func<T,U> map)
    {
        if (this.present)
        {
            return Optional.Of<U>(map.Invoke(this.value));
        } else
        {
            return Optional.Empty<U>();
        }
    }

    public T OrElse(T fallback)
    {
        if (this.present)
        {
            return this.value;
        } else
        {
            return fallback;
        }
    }

    public T OrElseGet(Func<T> fallbackSupplier)
    {
        if (this.present)
        {
            return this.value;
        }
        else
        {
            return fallbackSupplier.Invoke();
        }
    }

    public T OrElseThrow<E>(Func<E> exceptionSupplier) where E: Exception
    {
        if (this.present)
        {
            return this.value;
        }
        else
        {
            throw exceptionSupplier.Invoke();
        }
    }

    public void IfPresent(Action<T> action)
    {
        if (this.present)
        {
            action.Invoke(this.value);
        }
    }

    public bool IsEmpty()
    {
        return !this.present;
    }

    public bool IsPresent()
    {
        return this.present;
    }
}