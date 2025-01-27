namespace MDA;

public class MdaThrowable: Exception
{
    public MdaInstance Instance { get; set; }
    public Token? Token { get; set; }
    public string Message { get; set; }

    public MdaThrowable(MdaInstance instance, Token token, string message) : base(message)
    {
        Instance = instance;
        Token = token;
        Message = message;
    }
}