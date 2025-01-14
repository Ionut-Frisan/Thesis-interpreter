using System.Reflection;

namespace MDA;

/*
 * A delegate for a native method.
 */
public delegate object NativeMethodDelegate(Interpreter interpreter, MdaInstance instance,
    ICollection<object> arguments);


/*
 * This attribute is used to mark methods in a NativeClass that should be registered as a NativeMethod.
 */
[AttributeUsage(AttributeTargets.Method)]
public class NativeMethodAttribute : Attribute
{
    public string Name { get; }
    public int Arity { get; }

    public NativeMethodAttribute(string name, int arity)
    {
        Name = name;
        Arity = arity;
    }
}

/*
 * A native method that can be called from MDA code.
 */
public class NativeMethod : MdaFunction
{
    private readonly NativeMethodDelegate _method;
    private readonly int _arity;
    private MdaInstance? _instance;
    public readonly string Name;

    public NativeMethod(NativeMethodDelegate method, string name, int arity) : base(null, null, false)
    {
        Name = name;
        _method = method;
        _arity = arity;
    }

    public override object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return _method(interpreter, _instance, arguments);
    }
    
    public override NativeMethod Bind(MdaInstance instance)
    {
        _instance = instance;
        instance.Set("super", this);
        return this;
    }

    public override int Arity() => _arity;

    public override string ToString()
    {
        return $"<native method {Name}>";
    }
}

/*
 * Native classes that can be used in MDA code.
 */
public abstract class NativeClass : MdaClass
{
    public readonly string Name;
    private readonly MdaFunction? _initializer;
    private readonly IDictionary<string, NativeMethod> _methods = new Dictionary<string, NativeMethod>();

    protected NativeClass(string name) : base(name, null, new Dictionary<string, MdaFunction>())
    {
        Name = name;
        RegisterMethods();
        _initializer = FindMethod("init");
    }

    /*
     * Register all methods in the class that are marked with the NativeMethodAttribute.
     */
    private void RegisterMethods()
    {
        // Get all methods in the class that are marked with the NativeMethodAttribute.
        var methodInfos = GetType().GetMethods()
            .Where(m => m.GetCustomAttribute<NativeMethodAttribute>() != null);
        
        foreach (var methodInfo in methodInfos)
        {
            // Create a NativeMethod for each method and store it in the _methods dictionary.
            var attribute = methodInfo.GetCustomAttribute<NativeMethodAttribute>()!;
            var method = (NativeMethodDelegate) methodInfo.CreateDelegate(typeof(NativeMethodDelegate), this);
            _methods[attribute.Name] = new NativeMethod(method, attribute.Name, attribute.Arity);
        }
    }

    public override NativeMethod? FindMethod(string name)
    {
        if (_methods.ContainsKey(name))
        {
            return _methods[name];
        }

        return null;
    }

    public override object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        MdaInstance instance = new MdaInstance(this);
        if (_initializer != null)
        {
            _initializer.Bind(instance).Call(interpreter, arguments);
        }

        return instance;
    }

    public override int Arity()
    {
        return _initializer?.Arity() ?? 0;
    }

    public override string ToString() => $"<Class {Name}>";
}