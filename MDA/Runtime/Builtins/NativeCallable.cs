namespace MDA.Builtins;

public class NativeCallable: IMdaCallable
{
    private readonly NativeFunction _function;
    private readonly int _arity;
    
    public NativeCallable(NativeFunction function, int arity)
    {
        _function = function;
        _arity = arity;
    }
    
    public int Arity() => _arity;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return _function(interpreter, arguments);
    }
    
    public override string ToString() => $"<native fn {_function.Method.Name}>";
}