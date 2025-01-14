using MDA.Builtins;

namespace MDA;

public class MdaClass : IMdaCallable
{
    public readonly string Name;
    public readonly MdaClass? Superclass;
    private readonly IDictionary<string, MdaFunction> _methods;
    
    public MdaClass(string name,  MdaClass? superclass, IDictionary<string, MdaFunction> methods)
    {
        Name = name;
        Superclass = superclass;
        _methods = methods;
    }
    
    public MdaFunction? FindMethod(string name)
    {
        if (_methods.ContainsKey(name))
        {
            return _methods[name];
        }
        
        if (Superclass != null)
        {
            return Superclass.FindMethod(name);
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

    public virtual int Arity()  
    {
        MdaFunction? initializer = FindMethod("init");
        Console.WriteLine(initializer?.Arity());
        return initializer?.Arity() ?? 0;
    }
    
    public override string ToString() => $"<Class {Name}>";
    
    // Will be used by native classes
    public virtual bool HasMethod(string name) => false;
    
    // Will be used by native classes
    public virtual NativeMethodInfo GetMethod(string name) => 
        throw new RuntimeError(null, $"Undefined method '{name}'.");
}