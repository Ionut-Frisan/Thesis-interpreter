namespace MDA.ListMethods;

public class MdaListSorted: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListSorted(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 0;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.Sorted();
    }
    
    public override string ToString() => "<native fn list.sorted>";
}