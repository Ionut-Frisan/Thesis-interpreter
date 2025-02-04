namespace MDA;

public class Mda
{
    private static IExitHandler ExitHandler = new ExitHandler();
    private static IErrorReporter ErrorReporter = new ErrorReporter(ExitHandler.Exit);
    private static readonly Interpreter Interpreter = new Interpreter();
    
    public static void SetExitHandler(IExitHandler handler) => ExitHandler = handler;
    public static void SetErrorReporter(IErrorReporter reporter) => ErrorReporter = reporter;

    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: mda [script]");
            ExitHandler.Exit(1);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        var source = File.ReadAllBytes(path);
        var data = System.Text.Encoding.UTF8.GetString(source);

        if (String.IsNullOrWhiteSpace(data))
        {
            ErrorReporter.Error(0,0, "File is empty.");
            ExitHandler.Exit(35);
        }

        Run(data);

        // Indicate an error in the exit code.
        if (ErrorReporter.HadError) ExitHandler.Exit(65);
        if (ErrorReporter.HadRuntimeError) ExitHandler.Exit(70);
    }

    private static void RunPrompt()
    {
        var reader = new StreamReader(Console.OpenStandardInput());

        while (true)
        {
            Console.Write("> ");
            var line = reader.ReadLine();
            if (String.IsNullOrEmpty(line))
            {
                ExitHandler.Exit(1);
            }

            Run(line!);
            ErrorReporter.Reset();
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();
        
        // Stop if there was a syntax error.
        if (ErrorReporter.HadError) return;
        
        Optimizer optimizer = new Optimizer(statements);
        statements = optimizer.Optimize();
        
        // AstPrinter printer = new AstPrinter();
        // foreach (var statement in statements)
        // {
        //     Console.WriteLine(printer.Print(statement));
        // }
        
        // AstJsonSerializer serializer = new AstJsonSerializer();
        // Console.WriteLine(serializer.ToJson(statements));
        
        Resolver resolver = new Resolver(Interpreter);
        resolver.Resolve(statements);
        
        // Stop if there was a resolution error.
        if (ErrorReporter.HadError) return;

        try
        {
            Interpreter.Interpret(statements);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    }

    public static void Error(int line, int column, string message)
    {
        ErrorReporter.Error(line, column, message);
    }

    public static void Error(Token token, string message)
    {
        ErrorReporter.Error(token, message);
    }

    public static void RuntimeError(RuntimeError error)
    {
        ErrorReporter.RuntimeError(error);
    }

    public static void RuntimeError(MdaThrowable error)
    {
        ErrorReporter.RuntimeError(error);
    }
}