using System.Collections;

namespace MDA;

public class Environment
{
    public Environment? Enclosing { get; }
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
        if (_values.ContainsKey(name)) throw new RuntimeError(null, $"Variable '{name}' is already defined.");
        _values.Add(name, value);
    }
    
    public Environment Ancestor(int distance)
    {
        Environment environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment.Enclosing!;
        }

        return environment;
    }

    public object Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme]!;
        }
        
        if (Enclosing != null) return Enclosing.Get(name);
        
        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
    
    public object Get(string name)
    {
        if (_values.ContainsKey(name))
        {
            return _values[name]!;
        }
        
        if (Enclosing != null) return Enclosing.Get(name);
        
        throw new RuntimeError(null, $"Undefined variable '{name}'.");
    }
    
    public object GetAt(int distance, string name)
    {
        return Ancestor(distance)._values[name]!;
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

    public void AssignAt(int distance, Token name, object value)
    {
        Ancestor(distance)._values[name.Lexeme] = value;
    }
    
    public bool Contains(string name)
    {
        return _values.ContainsKey(name);
    }
}