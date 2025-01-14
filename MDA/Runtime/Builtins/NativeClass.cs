using System.Reflection;

namespace MDA.Builtins;

public delegate object NativeFunction(Interpreter interpreter, ICollection<object> arguments);

public delegate object NativeMethod(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments);

public abstract class NativeClass : MdaClass
{
    protected readonly Dictionary<string, NativeMethodInfo> _methods = new();
    public string Name { get; }
    private readonly NativeMethodInfo? _initializer;

    protected NativeClass(string name) : base(name, null, new Dictionary<string, MdaFunction>())
    {
        Name = name;
        RegisterMethods();
        if (HasMethod("init"))
        {
            _initializer = GetMethod("init");
        }
    }

    private void RegisterMethods()
    {
        var methodInfos = GetType().GetMethods()
            .Where(m => m.GetCustomAttribute<NativeMethodAttribute>() != null);

        foreach (var methodInfo in methodInfos)
        {
            var attr = methodInfo.GetCustomAttribute<NativeMethodAttribute>();
            _methods[attr.Name] = new NativeMethodInfo(
                (interpreter, instance, arguments) =>
                {
                    return methodInfo.Invoke(this, new object[] { interpreter, instance, arguments });
                },
                attr.Arity
            );
        }
    }

    public virtual object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        MdaInstance instance = new MdaInstance(this);
        if (_initializer != null)
        {
            _initializer.Method(interpreter, instance, arguments);
        }
        return instance;
    }
    
    public override int Arity()
    {
        return _initializer?.Arity ?? 0;
    }

    public override bool HasMethod(string name)
    {
        return _methods.ContainsKey(name);
    }

    public override NativeMethodInfo GetMethod(string name)
    {
        return _methods[name];
    }
}