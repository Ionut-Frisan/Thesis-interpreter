namespace MDA.Builtins;

public class ConsoleClass : NativeClass
{
    public ConsoleClass() : base("Console")
    { }
    
    [NativeMethod("print", 1)]
    public object Print(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        Console.Write(arguments.First());
        return null;
    }
    
    [NativeMethod("println", 1)]
    public object Println(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        Console.WriteLine(arguments.First());
        return null;
    }
    
    [NativeMethod("read", 0)]
    public object Read(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return Console.ReadLine();
    }
    
    [NativeMethod("readKey", 0)]
    public object ReadKey(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return Console.ReadKey().KeyChar;
    }
    
    [NativeMethod("clear", 0)]
    public object Clear(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        Console.Clear();
        return null;
    }
}