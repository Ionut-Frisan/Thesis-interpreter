namespace MDA;

public class Token
{
    public readonly TokenType Type;
    public readonly string Lexeme;
    public readonly object? Literal;
    public readonly int Line;
    public readonly int Column;
    
    public Token(TokenType type, string lexeme, object? literal, int line, int column)
    {
        this.Type = type;
        this.Lexeme = lexeme;
        this.Literal = literal;
        this.Line = line;
        this.Column = column;
    }

    public override String ToString()
    {
        return Type + " " + Lexeme + " " + Literal;
    }
}