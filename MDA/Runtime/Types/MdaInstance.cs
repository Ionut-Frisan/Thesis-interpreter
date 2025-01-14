namespace MDA;

public class MdaInstance
{
    private readonly MdaClass _klass;
    private readonly IDictionary<string, object> fields = new Dictionary<string, object>();
    
    public MdaInstance(MdaClass klass)
    {
        _klass = klass;
    }

    public object Get(Token name)
    {
        if (fields.ContainsKey(name.Lexeme))
        {
            return fields[name.Lexeme]!;
        }
        
        MdaFunction method = _klass.FindMethod(name.Lexeme);
        if (method != null) return method.Bind(this);
        
        throw new RuntimeError(name, $"Undefined property '{name.Lexeme}' on {this}'.");
    }
    
    public void Set(Token name, object value)
    {
        fields[name.Lexeme] = value;
    }
    
    public override string ToString() => $"<{_klass.Name} instance>";
}