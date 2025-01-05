namespace MDA;

public class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = [];
    private int _start;
    private int _current;
    private int _line = 1;
    private int _column = 1;
    private static Dictionary<string, TokenType> _keywords;

    public Scanner(string source)
    {
        _source = source;

        _keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "null", TokenType.NULL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line, _column));
        return _tokens;
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private void ScanToken()
{
    char c = Advance();
    switch (c)
    {
        case '(':
            AddToken(TokenType.LEFT_PAREN);
            _column++;
            break;
        case ')':
            AddToken(TokenType.RIGHT_PAREN);
            _column++;
            break;
        case '{':
            AddToken(TokenType.LEFT_BRACE);
            _column++;
            break;
        case '}':
            AddToken(TokenType.RIGHT_BRACE);
            _column++;
            break;
        case ',':
            AddToken(TokenType.COMMA);
            _column++;
            break;
        case '.':
            AddToken(TokenType.DOT);
            _column++;
            break;
        case '-':
            if (Match('-'))
            {
                AddToken(TokenType.MINUS_MINUS);
                _column += 2;
            }
            else
            {
                if (Match('='))
                {
                    AddToken(TokenType.MINUS_EQUAL);
                    _column += 2;
                }
                else
                {
                    AddToken(TokenType.MINUS);
                    _column++;
                }
            }
            break;
        case '+':
            if (Match('+'))
            {
                AddToken(TokenType.PLUS_PLUS);
                _column += 2;
            }
            else
            {
                if (Match('='))
                {
                    AddToken(TokenType.PLUS_EQUAL);
                    _column += 2;
                }
                else
                {
                    AddToken(TokenType.PLUS);
                    _column++;
                }
            }
            break;
        case ';':
            AddToken(TokenType.SEMICOLON);
            _column++;
            break;
        case '*':
            AddToken(Match('=') ? TokenType.STAR_EQUAL : TokenType.STAR);
            _column += Match('=') ? 2 : 1;
            break;
        case '%':
            AddToken(Match('=') ? TokenType.PERCENT_EQUAL : TokenType.PERCENT);
            _column += Match('=') ? 2 : 1;
            break;
        case '!':
            AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
            _column += Match('=') ? 2 : 1;
            break;
        case '=':
            AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
            _column += Match('=') ? 2 : 1;
            break;
        case '<':
            AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
            _column += Match('=') ? 2 : 1;
            break;
        case '>':
            AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
            _column += Match('=') ? 2 : 1;
            break;
        case '/':
            if (Match('/'))
            {
                while (Peek() != '\n' && !IsAtEnd()) Advance();
                _column += _current - _start;
            }
            else
            {
                if (Match('='))
                {
                    AddToken(TokenType.SLASH_EQUAL);
                    _column += 2;
                }
                else
                {
                    AddToken(TokenType.SLASH);
                    _column++;
                }
            }

            break;
        case ' ':
        case '\r':
        case '\t':
            _column++;
            break;
        case '\n':
            _line++;
            _column = 1;
            break;
        case '"':
            ProcessString();
            _column += _current - _start;
            break;
        default:
            if (IsDigit(c))
            {
                ProcessNumber();
                _column += _current - _start;
            }
            else if (IsAlpha(c))
            {
                Identifier();
                _column += _current - _start;
            }
            else
            {
                Mda.Error(_line, _column, $"Unrecognized character '{c}'");
                _column++;
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

        string text = _source.Substring(_start, _current - _start);
        TokenType type = _keywords.ContainsKey(text) ? _keywords[text] : TokenType.IDENTIFIER;

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

        AddToken(TokenType.NUMBER, Double.Parse(_source.Substring(_start, _current - _start)));
    }

    private void ProcessString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Mda.Error(_line, _column, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        string value = _source.Substring(_start + 1, _current - _start - 2);

        AddToken(TokenType.STRING, value);
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return _source[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length) return '\0';
        return _source[_current + 1];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_current] != expected) return false;

        _current++;
        return true;
    }

    private char Advance()
    {
        return _source[_current++];
    }

    private void AddToken(TokenType token)
    {
        AddToken(token, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        String text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line, _column));
    }
}