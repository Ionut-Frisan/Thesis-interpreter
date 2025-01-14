namespace MDA.Builtins;


public class ClockFunction : IMdaCallable
{
    // Override the Arity method to return 0.
    public int Arity() => 0;

    // Implement the Call method to return the current timestamp.
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    // Override ToString for representation.
    public override string ToString() => "<native function>";
}