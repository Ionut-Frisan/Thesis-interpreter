namespace MDA.ListMethods;

public class MdaListToString: IMdaListMethod
{
    public MdaList List { get; set; }

    public MdaListToString(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 0;

    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.ToString();
    }
}