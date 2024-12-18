using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace MDA;

public class Parser
{
    private class ParseError: Exception {}
    
    private List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        this._tokens = tokens;
    }

    public Expr Parse()
    {
        try
        {
            return Expression();
        }
        // Syntax error recovery is the parser’s job, so we don’t want the ParseError exception to escape into the rest of the interpreter.
        catch (ParseError e)
        {
            return null;
        }
    }

    private Expr Expression()
    {
        return Equality();
    }
    
    /*
     * equality       → comparison ( ( "!=" | "==" ) comparison )* ;
     */
    private Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token op = Previous();
            Expr right = Comparison();
            expr = new Expr.Binary(expr, op, right);
        }
        
        return expr;
    }
    
    /*
     
     * comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
     */
    private Expr Comparison()
    {
        Expr expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new Expr.Binary(expr, op, right);
        }
        
        return expr;
    }
    
    /*
     * term           → factor ( ( "-" | "+" ) factor )* ;
     */
    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            Expr right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }
    
    /*
     * factor         → unary ( ( "/" | "*" ) unary )* ;
     */
    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = Previous();
            Expr right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }
        
        return expr;
    }
    
    /*
     * unary          → ( "!" | "-" ) unary
     *                  | primary ;
     */
    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = Previous();
            Expr right = Unary();
            return new Expr.Unary(op, right);
        }
        
        return Primary();
    }
    
    /*
     * primary        → NUMBER | STRING | "true" | "false" | "null"
     *                  | "(" expression ")" ;
     */
    private Expr Primary()
    {
        if (Match(TokenType.FALSE)) return new Expr.Literal(false);
        if (Match(TokenType.TRUE)) return new Expr.Literal(true);
        if (Match(TokenType.NULL)) return new Expr.Literal(null);

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Literal(expr);
        }
        
        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenType tokenType, string message)
    {
        if (Check(tokenType)) return Advance();
        
        throw Error(Peek(), message);
    }
    
    /*
     * Report a ParseError.
     */
    private ParseError Error(Token token, string message)
    {
        Mda.Error(token, message);
        return new ParseError();
    }
    
    /*
     * Discard tokens until a statement boundary is found.
     * Used to recover from panic mode.
     */
    private void Syncronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
    
    
    /*
     * Checks if current token has any of the given types
     */
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }
    
    /*
     * Returns true if the current token is of the given type.
     * Unlike Match() it doesn't consume the token, it only looks at it.
     */
    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }
    
    /*
     * Consume the current token and returns it.
     */
    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }
    
    /*
     * Returns true if the current token is the last one.
     */
    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }
    
    /*
     * Returns the current token that is not yet consumed.
     */
    private Token Peek()
    {
        return _tokens[_current];
    }
    
    /*
     * Returns the most recent consumed token.
     */
    private Token Previous()
    {
        return _tokens[_current - 1];
    }
}