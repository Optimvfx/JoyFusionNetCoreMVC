namespace Common.Models;

public struct Procent
{
    public static double Max => 1f;
    public static double Min => 0;
    
    public static Procent Half => new Procent(0.5f);
    public static Procent Quarter => new Procent(0.25f);

    public readonly double Value;

    public Procent(double value)
    {
        if (value > Max || value < Min)
            throw new ArgumentException();
        
        Value = value;
    }
}