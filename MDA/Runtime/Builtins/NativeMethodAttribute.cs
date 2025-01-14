namespace MDA.Builtins;

public class NativeMethodAttribute: Attribute
{
    public string Name { get; }
    public int Arity { get; }
    
    public NativeMethodAttribute(string name, int arity)
    {
        Name = name;
        Arity = arity;
    }
}