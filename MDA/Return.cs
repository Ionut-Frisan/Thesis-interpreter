namespace MDA;

public class Return : Exception
{
    public object Value;

    public Return(object value) : base()
    {
        this.Value = value;
    }
}