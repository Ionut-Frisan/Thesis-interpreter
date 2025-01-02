namespace MDA;

public interface IMdaCallable
{
    public int Arity();
    public object Call(Interpreter interpreter, ICollection<object> arguments);
}