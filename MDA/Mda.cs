namespace MDA;

class Mda
{
    private static readonly Interpreter Interpreter = new Interpreter();
    private static bool _hadError;
    private static bool _hadRuntimeError;

    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: mda [script]");
            System.Environment.Exit(1);
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
            _hadError = true;
            System.Environment.Exit(1);
        }

        Run(data);

        // Indicate an error in the exit code.
        if (_hadError) System.Environment.Exit(65);
        if (_hadRuntimeError) System.Environment.Exit(70);
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
                System.Environment.Exit(1);
            }

            Run(line);
            _hadError = false;
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();
        
        Optimizer optimizer = new Optimizer(statements);
        statements = optimizer.Optimize();

        // Stop if there was a syntax error.
        if (_hadError) return;
        
        AstPrinter printer = new AstPrinter();
        foreach (var statement in statements)
        {
            Console.WriteLine(printer.Print(statement));
        }
        
        Resolver resolver = new Resolver(Interpreter);
        resolver.Resolve(statements);
        
        // Stop if there was a resolution error.
        if (_hadError) return;

        Interpreter.Interpret(statements);
    }

    public static void Error(int line, int column, string message)
    {
        Report(line, column, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, token.Column, "at end", message);
        }
        else
        {
            Report(token.Line, token.Column, "at", "'" + token.Lexeme + "' " + message);
        }
    }

    private static void Report(int line, int column, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}:{column}] Error {where}: {message}");
        _hadError = true;
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"[line {error.Token.Line}:{error.Token.Column}] {error.Message}");
        _hadRuntimeError = true;
    }
}