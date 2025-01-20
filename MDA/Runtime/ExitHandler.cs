namespace MDA;

public class ExitHandler: IExitHandler
{
    public void Exit(int exitCode)
    {
        System.Environment.Exit(exitCode);
    }
}