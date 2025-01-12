namespace MDA.ListMethods;

public class MdaListSort: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListSort(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 0;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        List.Sort();
        return List;
    }
    
    public override string ToString() => "<native fn list.sort>";
    
}