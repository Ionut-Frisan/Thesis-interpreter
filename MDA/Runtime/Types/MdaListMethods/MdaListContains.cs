namespace MDA.ListMethods;

public class MdaListContains: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListContains(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 1;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.Contains(arguments.First());
    }
    
    public override string ToString() => "<native fn list.contains>";
    
}