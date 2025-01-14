using MDA.Builtins;

namespace MDA;

public class MdaInstance
{
    private readonly MdaClass _klass;
    private readonly IDictionary<string, object> _fields = new Dictionary<string, object>();
    
    public MdaInstance(MdaClass klass)
    {
        _klass = klass;
    }

    public object Get(Token name)
    {
        if (_fields.ContainsKey(name.Lexeme))
        {
            return _fields[name.Lexeme]!;
        }

        if (_klass.HasMethod(name.Lexeme))
        {
            return new BoundMethod(this, _klass.GetMethod(name.Lexeme));
        }
        
        MdaFunction method = _klass.FindMethod(name.Lexeme);
        if (method != null) return method.Bind(this);
        
        throw new RuntimeError(name, $"Undefined property '{name.Lexeme}' on {this}'.");
    }
    
    public void Set(Token name, object value)
    {
        _fields[name.Lexeme] = value;
    }
    
    public override string ToString() => $"<{_klass.Name} instance>";
}