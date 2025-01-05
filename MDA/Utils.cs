namespace MDA;

public class Utils
{
    public static string Stringify(object? value)
    {
        if (value == null) return "null";
        if (value is double)
        {
            string text = ((double)value).ToString();

            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return value.ToString();
    }
}