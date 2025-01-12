namespace MDA.ListMethods;

public class MdaListPop: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListPop(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 0;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.Pop();
    }
    
    public override string ToString() => "<native fn list.pop>";
}