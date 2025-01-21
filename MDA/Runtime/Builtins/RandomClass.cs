namespace MDA.Builtins;

public class RandomClass: NativeClass
{
    public RandomClass() : base("Random")
    {
        
    }
    
    [NativeMethod("init", 0)]
    public object Init(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return instance;
    }
    
    [NativeMethod("range", 2)]
    public object Range(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return new Random().Next((int)(double)arguments.First(), (int)(double)arguments.Last());
    }
    
    [NativeMethod("next", 0)]
    public object Next(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        return new Random().NextDouble();
    }
}