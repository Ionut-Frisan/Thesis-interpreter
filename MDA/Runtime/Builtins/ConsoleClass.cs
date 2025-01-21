namespace MDA.Builtins;

public class ConsoleClass: NativeClass
{
    public ConsoleClass() : base("Console")
    { }
    
    [NativeMethod("init", 0)]
    public object Init(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return instance;
    }
    
    [NativeMethod("write", 1)]
    public object Write(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        Console.Write(Utils.Stringify(arguments.First()));
        return null;
    }    
    
    [NativeMethod("writeLine", 1)]
    public object WriteLine(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        Console.WriteLine(Utils.Stringify(arguments.First()));
        return null;
    }
    
    [NativeMethod("readLine", 0)]
    public object ReadLine(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return Console.ReadLine();
    }
    
    [NativeMethod("readKey", 0)]
    public object ReadKey(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return Console.ReadKey();
    }
    
    [NativeMethod("clear", 0)]
    public object Clear(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        Console.Clear();
        return null;
    }
}