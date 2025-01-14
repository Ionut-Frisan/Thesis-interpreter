using System.Security.Cryptography;

namespace MDA;

public delegate object NativeFunctionDelegate(Interpreter interpreter, ICollection<object> arguments);

[AttributeUsage(AttributeTargets.Method)]
public class NativeFunctionAttribute : Attribute
{
    public int Arity { get; set; }
    public string Name { get; set; }

    public NativeFunctionAttribute(string name, int arity)
    {
        Name = name;
        Arity = arity;
    }
}

public class NativeFunction: IMdaCallable
{
    private int _arity { get; set; }
    private string _name { get; set; }
    public NativeFunctionDelegate Function { get; set; }
    
    public NativeFunction(NativeFunctionDelegate function, string Name, int arity)
    {
        Function = function;
        _name = Name;
        _arity = arity;
    }
    
    public object? Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return Function(interpreter, arguments);
    }
    
    public int Arity() => _arity;

    public override string ToString() => $"<native function {_name}>";
}