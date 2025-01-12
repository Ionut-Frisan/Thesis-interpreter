namespace MDA.ListMethods;

public class MdaListPush: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListPush(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 1;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        object value = arguments.First();
        List.Push(value);
        return null;
    }
    
    public override string ToString() => "<native fn list.push>";
}