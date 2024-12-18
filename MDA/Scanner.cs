namespace MDA;

// TODO: add support for multi line comments /* ... */
// TODO: add support for ternary operator ?:
// TODO: add support for arrays
// TODO: add support for import/packages

public class Scanner
{
    private string source;
    private List<Token> tokens = [];
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private static Dictionary<string, TokenType> keywords;

    public Scanner(string source)
    {
        this.source = source;

        keywords = new Dictionary<string, TokenType>();
        keywords.Add("and", TokenType.AND);
        keywords.Add("class", TokenType.CLASS);
        keywords.Add("else", TokenType.ELSE);
        keywords.Add("false", TokenType.FALSE);
        keywords.Add("for", TokenType.FOR);
        keywords.Add("fun", TokenType.FUN);
        keywords.Add("if", TokenType.IF);
        keywords.Add("null", TokenType.NULL);
        keywords.Add("or", TokenType.OR);
        keywords.Add("print", TokenType.PRINT);
        keywords.Add("return", TokenType.RETURN);
        keywords.Add("super", TokenType.SUPER);
        keywords.Add("this", TokenType.THIS);
        keywords.Add("true", TokenType.TRUE);
        keywords.Add("var", TokenType.VAR);
        keywords.Add("while", TokenType.WHILE);
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }

                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            case '\n':
                line++;
                break;
            case '"': ProcessString(); break;

            default:
                if (IsDigit(c))
                {
                    ProcessNumber();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Mda.Error(line, $"Unrecognized character '{c}'");
                }

                break;
        }
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsDigit(c) || IsAlpha(c);
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = source.Substring(start, current - start);
        TokenType type = keywords.ContainsKey(text) ? keywords[text] : TokenType.IDENTIFIER;

        AddToken(type);
    }

    private void ProcessNumber()
    {
        while (IsDigit(Peek())) Advance();
        
        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the "."
            Advance();

            while (IsDigit(Peek())) Advance();
        }
        
        AddToken(TokenType.NUMBER, Double.Parse(source.Substring(start, current - start)));
    }

    private void ProcessString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Mda.Error(line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        string value = source.Substring(start + 1, current - start - 2);

        AddToken(TokenType.STRING, value);
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private char Advance()
    {
        return source[current++];
    }

    private void AddToken(TokenType token)
    {
        AddToken(token, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        String text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }
}