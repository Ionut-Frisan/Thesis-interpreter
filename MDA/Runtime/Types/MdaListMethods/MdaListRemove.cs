namespace MDA.ListMethods;

public class MdaListRemove : IMdaListMethod
{
    public MdaList List { get; set; }

    public MdaListRemove(MdaList list)
    {
        List = list;
    }

    public int Arity() => 1;

    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.Remove(arguments.First());
    }

    public override string ToString() => "<native fn list.remove>";
}