using System.Diagnostics;
using System.Reflection;

namespace MDA.Builtins;

public static class NativeFunctions
{
    public static Dictionary<string, NativeFunction> GetAllNativeFunctions()
    {
        var methodInfos = typeof(NativeFunctions).GetMethods().Where((methodInfo) =>
        {
            return methodInfo.GetCustomAttribute<NativeFunctionAttribute>() != null;
        });

        var nativeFunctionRegistry = new Dictionary<string, NativeFunction>();

        foreach (var methodInfo in methodInfos)
        {
            var attr = methodInfo.GetCustomAttribute<NativeFunctionAttribute>();
            var nativeFunctionDelegate =
                (NativeFunctionDelegate)Delegate.CreateDelegate(typeof(NativeFunctionDelegate), methodInfo);

            nativeFunctionRegistry.Add(attr.Name, new NativeFunction(nativeFunctionDelegate, attr.Name, attr.Arity));
        }

        return nativeFunctionRegistry;
    }

    [NativeFunction("typeof", 1)]
    public static object TypeOf(Interpreter interpreter, ICollection<object> arguments)
    {
        object? value = arguments.FirstOrDefault();

        if (value == null) return "null";
        if (value is string) return "string";
        if (value is double) return "number";
        if (value is bool) return "bool";
        if (value is MdaClass klass) return $"Class<{klass.Name}>";
        if (value is MdaInstance instance) return $"Instance<{instance.Klass.Name}>";
        if (value is MdaFunction function) return "function";
        if (value is MdaList) return "List";
        return value.GetType().Name;
    }

    [NativeFunction("is", 2)]
    public static object Is(Interpreter interpreter, ICollection<object> arguments)
    {
        object? value = arguments.FirstOrDefault();
        object? type = arguments.LastOrDefault();

        if (type is MdaClass klass)
        {
            return value is MdaInstance instance && (instance.Klass == klass || instance.Klass?.Superclass == klass);
        }

        if (value is MdaClass klassValue)
        {
            if (type is string klasstype)
            {
                if (klassValue.Name == klasstype || klasstype == "class") return true;
            }
        }
        
        if (type is string stype)
        {
            return (string)TypeOf(interpreter, new List<object>() { value }) == stype;
        }

        return false;
    }

    [NativeFunction("clock", 0)]
    public static object Clock(Interpreter interpreter, ICollection<object> arguments) =>
        (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}