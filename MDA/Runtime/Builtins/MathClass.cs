namespace MDA.Builtins;

public class MathClass: NativeClass
{
    public MathClass() : base("Math") { }

    [NativeMethod("square", 1)]
    public object Square(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments)
    {
        double arg = (double) arguments.First();
        return arg * arg;
    }
    
    [NativeMethod("sqrt", 1)]
    public object Sqrt(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Sqrt((double) arguments.First());

    [NativeMethod("abs", 1)]
    public object Abs(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Abs((double) arguments.First());
    
    [NativeMethod("floor", 1)]
    public object Floor(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Floor((double) arguments.First());
    
    [NativeMethod("ceil", 1)]
    public object Ceil(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Ceiling((double) arguments.First());
    
    [NativeMethod("round", 1)]
    public object Round(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Round((double) arguments.First());
    
    [NativeMethod("pow", 2)]
    public object Pow(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Pow((double) arguments.First(), (double) arguments.ElementAt(1));
    
    [NativeMethod("min", 2)]
    public object Min(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Min((double) arguments.First(), (double) arguments.ElementAt(1));
    
    [NativeMethod("max", 2)]
    public object Max(Interpreter interpreter, MdaInstance instance, ICollection<object> arguments) => Math.Max((double) arguments.First(), (double) arguments.ElementAt(1));
}