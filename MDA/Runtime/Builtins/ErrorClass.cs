namespace MDA.Builtins;

public class ErrorClass: NativeClass
{
    public ErrorClass() : base("Error")
    {
    }

    [NativeMethod("init", 1)]
    public object Init(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        instance.Set("message", arguments.First());
        return instance;
    }
}