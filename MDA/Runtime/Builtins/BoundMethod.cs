using System.Reflection;

namespace MDA.Builtins;

public class BoundMethod: IMdaCallable
{
    private readonly MdaInstance _instance;
    private readonly NativeMethodInfo _methodInfo;
    
    public BoundMethod(MdaInstance instance, NativeMethodInfo methodInfo)
    {
        _instance = instance;
        _methodInfo = methodInfo;
    }

    public int Arity() => _methodInfo.Arity;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return _methodInfo.Method(interpreter, _instance, arguments);
    }
}