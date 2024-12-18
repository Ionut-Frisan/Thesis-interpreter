namespace MDA;

class Mda
{
    static bool hadError = false;
    
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: mda [script]");
            System.Environment.Exit(1);
        } else if (args.Length == 1)
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
        var data = source.ToString();
        
        if (String.IsNullOrWhiteSpace(data))
        {
            hadError = true;
            System.Environment.Exit(1);
        }

        Run(data);
        
        // Indicate an error in the exit code.
        if (hadError) System.Environment.Exit(65);
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
            hadError = false;
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        
        Parser parser = new Parser(tokens);
        Expr expr = parser.Parse();

        if (hadError) return;
        
        Console.WriteLine(new AstPrinter().Print(expr));
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at ", token.Lexeme + "'" + message);
        }
    }

    private static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error {where}: {message}");
    }
}
