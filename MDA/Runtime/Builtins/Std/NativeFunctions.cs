namespace MDA.Builtins;

public class NativeFunctions
{
    [NativeFunction("clock", 0)]
    public static object Clock(Interpreter interpreter, ICollection<object> arguments)
    {
        return (double)(DateTime.Now.Ticks / 10000);
    }
}