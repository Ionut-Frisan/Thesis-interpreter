namespace MDA.ListMethods;

public interface IMdaListMethod: IMdaCallable
{
    public abstract MdaList List { get; set; }
}