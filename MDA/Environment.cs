using System.Collections;

namespace MDA;

public class Environment
{
    private Environment? Enclosing { get; set; }
    private readonly Hashtable _values = new Hashtable();

    public Environment()
    {
        Enclosing = null;
    }
    
    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }

    public void Define(string name, object? value)
    {
        // TODO: might want to check if the name already is in the map so that redefining of variables is not allowed
        _values.Add(name, value);
    }

    public object Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme];
        }
        
        if (Enclosing != null) return Enclosing.Get(name);
        
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }
        
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}