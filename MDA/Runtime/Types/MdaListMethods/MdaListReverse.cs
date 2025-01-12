namespace MDA.ListMethods;

public class MdaListReverse: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListReverse(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 0;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        List.Reverse();
        return List;
    }
    
    public override string ToString() => "<native fn list.reverse>";
}