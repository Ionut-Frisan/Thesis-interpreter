namespace MDA;

public class MdaFunction : IMdaCallable
{
    private readonly Stmt.Function _declaration;
    private readonly Environment _closure;

    public MdaFunction(Stmt.Function declaration, Environment closure)
    {
        _declaration = declaration;
        _closure = closure;
    }

    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        Environment environment = new Environment(_closure);
        for (int i = 0; i < _declaration.Parameters.Count; i++)
        {
            environment.Define(_declaration.Parameters.ElementAt(i).Lexeme, arguments.ElementAt(i));
        }

        try
        {
            interpreter.ExecuteBlock(_declaration.Body, environment);
        }
        catch (Return returnValue)
        {
            return returnValue.Value;
        }
        return null;
    }
    
    public int Arity() => _declaration.Parameters.Count;
    
    public override string ToString() => $"<fn {_declaration.Name.Lexeme}>";
}