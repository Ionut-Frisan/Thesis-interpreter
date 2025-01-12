namespace MDA.ListMethods;

public class MdaListInsertAt: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListInsertAt(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 2;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        if (arguments.First() is double == false)
        {
            throw new RuntimeError(null, "First argument must be a number.");
        }
        
        int index = (int)((double) arguments.First());
        object value = arguments.ElementAt(1);
        List.InsertAt(index, value);
        return null;
    }
    
    public override string ToString() => "<native fn list.insert>";
}