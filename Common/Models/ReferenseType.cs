namespace Common.Models;

public class ReferenseType<T>
    where T : struct

{
    public T Value { get; set; }

    public ReferenseType(T value)
    {
        Value = value;
    }
}