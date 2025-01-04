namespace MDA;

public class MdaClass : IMdaCallable
{
    public readonly string Name;
    private readonly IDictionary<string, MdaFunction> _methods;
    
    public MdaClass(string name, IDictionary<string, MdaFunction> methods)
    {
        Name = name;
        _methods = methods;
    }
    
    public MdaFunction? FindMethod(string name)
    {
        if (_methods.ContainsKey(name))
        {
            return _methods[name];
        }
        return null;
    }
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        MdaInstance instance = new MdaInstance(this);
        MdaFunction initializer = FindMethod("init");
        if (initializer != null)
        {
            initializer.Bind(instance).Call(interpreter, arguments);
        }
        
        return instance;
    }

    public int Arity()
    {
        MdaFunction? initializer = FindMethod("init");
        return initializer?.Arity() ?? 0;
    }
    
    public override string ToString() => $"<Class {Name}>";
}