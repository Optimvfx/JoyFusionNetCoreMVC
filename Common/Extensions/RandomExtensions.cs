using System.Text;
using Common.Models;

namespace Common.Extensions;

public static class RandomExtensions
{
    public static bool NextBool(this Random rand, Procent? trueProcent = null)
    {
        var procent = trueProcent ?? Procent.Half;
        return rand.NextDouble() < procent.Value;
    }
    
    public static string NextString(this Random random, int length, Procent? emptyProcent = null)
    {
        emptyProcent = emptyProcent ?? new Procent(Procent.Min);
        
        if (length < 0)
            throw new AggregateException();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            if (random.NextBool(emptyProcent))
            {
                stringBuilder.Append(' ');
                continue;
            }
            
            int index = random.Next(chars.Length);
            char randomChar = chars[index];
            stringBuilder.Append(randomChar);
        }

        return stringBuilder.ToString();
    }
}