namespace MDA;

public class Token
{
    public TokenType Type;
    public String Lexeme;
    public Object Literal;
    public int Line;

    public Token(TokenType type, string lexeme, object literal, int line)
    {
        this.Type = type;
        this.Lexeme = lexeme;
        this.Literal = literal;
        this.Line = line;
    }

    public override String ToString()
    {
        return Type + " " + Lexeme + " " + Literal;
    }
}