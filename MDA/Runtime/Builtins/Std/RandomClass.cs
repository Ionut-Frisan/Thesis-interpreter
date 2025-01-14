namespace MDA.Builtins;

public class RandomClass: NativeClass
{
    public RandomClass(): base("Random")
    { }
    
    [NativeMethod("range", 2)]
    public object Range(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        if (arguments.Count != 2)
        {
            throw new RuntimeError(null, "Expected 2 arguments");
        }
        
        if (!(arguments.ElementAt(0) is double) || !(arguments.ElementAt(1) is double))
        {
            throw new RuntimeError(null, "Expected 2 numbers");
        }
        
        int min = (int)(double) arguments.ElementAt(0);
        int max = (int)(double) arguments.ElementAt(1);
        
        return new Random().Next(min, max);
    }
}