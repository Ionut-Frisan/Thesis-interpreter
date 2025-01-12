using System.Collections.ObjectModel;

namespace MDA.ListMethods;

public class MdaListLength: IMdaListMethod
{
    public MdaList List { get; set; }
    
    public MdaListLength(MdaList list)
    {
        List = list;
    }
    
    public int Arity() => 0;
    
    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return (double)List.Length();
    }

    public override string ToString() => "<native fn list.length>";
}