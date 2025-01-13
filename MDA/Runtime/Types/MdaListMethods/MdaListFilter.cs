namespace MDA.ListMethods;

public class MdaListFilter: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListFilter(MdaList list)
    {
        List = list;    
    }
    
    public int Arity() => 1;

    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        if (!(arguments.First() is MdaFunction))
        {
            throw new RuntimeError(null, "First argument must be a function.");
        }

        MdaFunction function = (MdaFunction)arguments.First();
        return List.Filter(interpreter, function);
    }
}