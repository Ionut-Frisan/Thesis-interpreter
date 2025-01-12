namespace MDA.ListMethods;

public class MdaListRemoveAt: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListRemoveAt(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 1;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        if (!(arguments.First() is double))
        {
            throw new RuntimeError(null, "Expected argument to be a number.");
        }
        
        double index = (double) arguments.First();
        
        List.RemoveAt((int) index);
        return null;
    }
    
    public override string ToString() => "<native fn list.removeAt>";
    
}