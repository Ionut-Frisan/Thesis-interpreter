namespace MDA.Builtins;

public class NativeFunctionAttribute: Attribute
{
    public string Name { get; }
    public int Arity { get; }
    
    public NativeFunctionAttribute(string name, int arity)
    {
        Name = name;
        Arity = arity;
    }
}