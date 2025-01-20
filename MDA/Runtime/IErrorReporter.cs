namespace MDA;

public interface IErrorReporter
{
    bool HadError { get; }
    bool HadRuntimeError { get; }
    void Error(Token token, string message);
    void Error(int line, int column, string message);
    void RuntimeError(RuntimeError error);
    void Reset();
}