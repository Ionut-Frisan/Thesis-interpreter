using System.Collections;

namespace MDA;

public class Environment
{
    private Hashtable values = new Hashtable();

    public void Define(string name, object? value)
    {
        // TODO: might want to check if the name already is in the map so that redefining of variables is not allowed
        values.Add(name, value);
    }

    public object Get(Token name)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            return values[name.Lexeme];
        }
        
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            values[name.Lexeme] = value;
            return;
        }
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}