using System.Text;

namespace Common.Extensions;

public static class RandomExtensions
{
    public static bool NextBool(this Random rand)
    {
        return rand.Next(2) == 0;
    }
    
    public static string NextString(this Random random, int length)
    {
        if (length < 0)
            throw new AggregateException();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length);
            char randomChar = chars[index];
            stringBuilder.Append(randomChar);
        }

        return stringBuilder.ToString();
    }
}