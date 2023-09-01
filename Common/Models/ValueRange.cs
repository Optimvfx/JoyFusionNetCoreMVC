namespace Common.Models;

public struct ValueRange<T> 
where T : struct, IComparable<T>
{
    public readonly T Min;
    public readonly T Max;

    public ValueRange(T min, T max)
    {
        if (min.CompareTo(max) > 0)
            throw new ArgumentException();
        
        Max = max;
        Min = min;
    }

    public bool OutOfRange(T value)
    {
        return !InRange(value);
    }
    
    public bool InRange(T value)
    {
        return value.CompareTo(Max) <= 0 && value.CompareTo(Min) >= 0;
    }
}