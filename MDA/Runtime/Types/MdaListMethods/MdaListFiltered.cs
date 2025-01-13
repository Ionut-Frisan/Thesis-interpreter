namespace MDA.ListMethods;

public class MdaListFiltered: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListFiltered(MdaList list)
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
        return List.Filtered(interpreter, function);
    }
}