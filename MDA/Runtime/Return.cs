namespace MDA;

public class Return : Exception
{
    public readonly object? Value;

    public Return(object? value) : base()
    {
        this.Value = value;
    }
}