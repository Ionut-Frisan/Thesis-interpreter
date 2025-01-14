namespace MDA.Builtins;

public class NativeMethodInfo
{
    public NativeMethod Method { get; }
    public int Arity { get; }
    
    public NativeMethodInfo(NativeMethod method, int arity)
    {
        Method = method;
        Arity = arity;
    }
}