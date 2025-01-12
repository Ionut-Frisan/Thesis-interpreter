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

    public static bool IsTruthy(object? value)
    {
        if (value == null) return false;
        if (value is bool) return (bool)value;
        if (value is double) return (double)value != 0;
        if (value is string) return (string)value != "";
        return true;
    }

    /*
     * Compare two objects.
     *
     * For numbers we compare the values, for strings we compare the alphabetical order,
     * anything is considered greater than null and every other comparison is false.
     *
     * Returns true if a is greater than b.
     *
     */
    public static bool IsGreater(object? a, object? b)
    {
        if (a == null && b == null) return false;
        if (a == null) return false;
        if (b == null) return true;
        if (a is double da && b is double db) return da > db;
        if (a is string sa && b is string sb) return string.Compare(sa, sb) > 0;
        if (a is bool ba && b is bool bb) return ba && bb == false;
        return false;
    }
    
    /*
     * Compare two objects.
     *
     * For numbers we compare the values, for strings we compare the alphabetical order,
     * anything is considered greater than null and every other comparison is false.
     *
     * Returns true if a is less than b.
     *
     */
    public static bool IsLess(object? a, object? b)
    {
        if (a == null && b == null) return false;
        if (a == null) return true;
        if (b == null) return false;
        if (a is double da && b is double db) return da < db;
        if (a is string sa && b is string sb) return string.Compare(sa, sb) < 0;
        if (a is bool ba && b is bool bb) return ba == false && bb;
        return false;
    }
    
    /*
     * Compare two objects.
     *
     * Returns 1 if a is greater than b, -1 if a is less than b and 0 if they are equal.
     */
    public static int Compare(object? a, object? b)
    {
        if (a == null && b == null) return 0;
        if (a == null) return -1;
        if (b == null) return 1;
        if (a is double da && b is double db) return da > db ? 1 : da < db ? -1 : 0;
        if (a is string sa && b is string sb) return string.Compare(sa, sb) > 0 ? 1 : string.Compare(sa, sb) < 0 ? -1 : 0;
        if (a is bool ba && b is bool bb) return ba == bb ? 0 : ba ? 1 : -1;
        return 0;
    }
}