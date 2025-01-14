using System.Reflection;

namespace MDA.Builtins;

public static class NativeFunctionRegistry
{
    private static readonly Dictionary<string, (NativeFunction func, int arity)> _functions = new();

    public static void RegisterClass<T>() where T : NativeClass, new()
    {
        var instance = new T();
        _functions[instance.Name] = (
            (NativeFunction)((interpreter, args) => instance.Call(interpreter, args)),
            instance.Arity()
        );
    }

    public static IEnumerable<string> GetAllFunctionNames() => _functions.Keys;

    public static void RegisterFromType(Type type)
    {
        var methodsInfo = type.GetMethods()
            .Where(m => m.GetCustomAttribute<NativeFunctionAttribute>() != null);

        foreach (var methodInfo in methodsInfo)
        {
            var attr = methodInfo.GetCustomAttribute<NativeFunctionAttribute>();
            _functions[attr.Name] = (
                (interpreter, args) => methodInfo.Invoke(null, new object[] { interpreter, args }),
                attr.Arity
            );
        }
    }

    public static bool HasFunction(string name)
    {
        return _functions.ContainsKey(name);
    }

    public static (NativeFunction func, int arity) GetFunction(string name)
    {
        return _functions[name];
    }
}