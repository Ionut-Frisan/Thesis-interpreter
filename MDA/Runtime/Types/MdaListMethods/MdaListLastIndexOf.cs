namespace MDA.ListMethods;

public class MdaListLastIndexOf: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListLastIndexOf(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 1;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.LastIndexOf(arguments.First());
    }
    
    public override string ToString() => "<native fn list.lastIndexOf>";

    
}