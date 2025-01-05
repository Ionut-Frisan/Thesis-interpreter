namespace MDA;

public class MdaFunction : IMdaCallable
{
    private readonly Stmt.Function _declaration;
    private readonly Environment _closure;
    private readonly bool _isInitializer;

    public MdaFunction(Stmt.Function declaration, Environment closure, bool isInitializer = false)
    {
        _declaration = declaration;
        _closure = closure;
        _isInitializer = isInitializer;
    }
    
    public MdaFunction Bind(MdaInstance instance)
    {
        Environment environment = new Environment(_closure);
        environment.Define("this", instance);
        return new MdaFunction(_declaration, environment);
    }

    public object? Call(Interpreter interpreter, ICollection<object> arguments)
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
            if (_isInitializer) return _closure.GetAt(0, "this");
            
            return returnValue.Value;
        }
        
        // return the instance from the intializer
        if (_isInitializer) return _closure.GetAt(0, "this");
        return null;
    }
    
    public int Arity() => _declaration.Parameters.Count;
    
    public override string ToString() => $"<fn {_declaration.Name.Lexeme}>";
}