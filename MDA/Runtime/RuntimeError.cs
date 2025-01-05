namespace MDA;

public class RuntimeError : Exception
{
    public Token Token { get; }

    public RuntimeError(Token token, String message) : base(message)
    {
        this.Token = token;
    }
}