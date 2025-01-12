namespace MDA.ListMethods;

public class MdaListIndexOf: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListIndexOf(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 1;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.IndexOf(arguments.First());
    }
    
    public override string ToString() => "<native fn list.indexOf>";
}