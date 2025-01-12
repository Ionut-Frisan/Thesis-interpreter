namespace MDA.ListMethods;

public class MdaListRemoveAll : IMdaListMethod
{
    public MdaList List { get; set; }

    public MdaListRemoveAll(MdaList list)
    {
        List = list;
    }

    public int Arity() => 1;

    public object Call(Interpreter interpreter, ICollection<object> arguments)
    {
        return List.RemoveAll(arguments.First());
    }

    public override string ToString() => "<native fn list.removeAll>";
}