namespace MDA;

public class ErrorReporter: IErrorReporter
{
    public bool HadError { get; private set; }
    public bool HadRuntimeError { get; private set; }
    private readonly Action<int> _exitHandler;

    public ErrorReporter(Action<int>? exitHandler = null)
    {
        _exitHandler = exitHandler ?? System.Environment.Exit;
    }

    public void Error(Token token, string message)
    {
        HadError = true;
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, token.Column, "at end", message);
        }
        else
        {
            Report(token.Line, token.Column, "at", "'" + token.Lexeme + "' " + message);
        }
    }
    
    public void Error(int line, int column, string message)
    {
        HadError = true;
        Report(line, column, "", message);
    }

    public void RuntimeError(RuntimeError error)
    {
        HadRuntimeError = true;
        Token? token = error.Token;
        if (token == null)
        {
            Console.Error.WriteLine($"[Runtime Error]: {error.Message}");
        }
        else
        {
            Console.Error.WriteLine($"[line {token.Line}:{token.Column}] {error.Message}");
        }

        _exitHandler(70);
    }

    public void Reset()
    {
        HadError = false;
        HadRuntimeError = false;
    }

    private void Report(int line, int column, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}:{column}] Error {where}: {message}");
        _exitHandler(65);
    }
}