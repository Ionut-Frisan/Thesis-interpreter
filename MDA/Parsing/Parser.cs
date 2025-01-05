namespace MDA;

public class Parser
{
    private class ParseError : Exception
    {
    }

    private List<Token> _tokens;
    private int _current;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt? Declaration()
    {
        try
        {
            if (Match(TokenType.CLASS)) return ClassDeclaration();
            if (Match(TokenType.FUN)) return Function("function");
            if (Match(TokenType.VAR)) return VarDeclaration();

            return Statement();
        }
        catch (ParseError)
        {
            Syncronize();
            return null;
        }
    }

    private Stmt ClassDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect class name.");
        
        Expr.Variable superclass = null;
        if (Match(TokenType.LESS))
        {
            Consume(TokenType.IDENTIFIER, "Expect superclass name.");
            superclass = new Expr.Variable(Previous());
        }
        
        Consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");
        
        List<Stmt.Function> methods = new List<Stmt.Function>();
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            methods.Add(Function("method"));
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");

        return new Stmt.Class(name, superclass, methods);
    }

    private Stmt Statement()
    {
        if (Match((TokenType.FOR))) return ForStatement();
        if (Match(TokenType.IF)) return IfStatement();
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Match(TokenType.RETURN)) return ReturnStatement();
        if (Match(TokenType.WHILE)) return WhileStatement();
        if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

        return ExpressionStatement();
    }

    private Stmt ForStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after 'for'.");
        
        Stmt? initializer;
        if (Match(TokenType.SEMICOLON))
        {   
            // Initializer can be omitted.
            // for( ; <condition> ; <increment>)
            initializer = null;
        } else if (Match(TokenType.VAR))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        Expr? condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expected ';' after 'for' condition.");
        
        Expr? increment = null;
        if (!Check(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after 'for' clauses.");
        
        Stmt body = Statement();
        
        // Since for is just syntactic sugar over a while, we desugarize the for statement into a while statement
        // for (<initializer>; <condition>; <increment> becomes
        // <initializer>; while (<condiiton>) { ...bodu, <increment> };
        if (increment != null)
        {
            body = new Stmt.Block(new List<Stmt>
            {
                body,
                new Stmt.Expression(increment)
            });
        }
        
        if (condition == null) condition = new Expr.Literal(true);
        body = new Stmt.While(condition, body);

        if (initializer != null)
        {
            body = new Stmt.Block(new List<Stmt> { initializer, body });
        }
        
        return body;
    }
    
    private Stmt VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expected variable name.");

        Expr? initilizer = null;
        if (Match(TokenType.EQUAL))
        {
            initilizer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration.");
        return new Stmt.Var(name, initilizer);
    }

    private Stmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after 'while'.");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after 'while'.");
        Stmt body = Statement();
        
        return new Stmt.While(condition, body);
    }

    private Stmt IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expected '(' after if.");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after if condition.");
        
        Stmt thenBranch = Statement();
        Stmt? elseBranch = null;
        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }
        
        return new Stmt.If(condition, thenBranch, elseBranch);
    }
    
    private Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt ReturnStatement()
    {
        Token keyword = Previous();
        Expr? value = null;
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }
        
        Consume(TokenType.SEMICOLON, "Expected ';' after return value.");
        return new Stmt.Return(keyword, value);
    }

    private Stmt ExpressionStatement()
    {
        Expr expression = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new Stmt.Expression(expression);
    }

    // Same function will be used for classes and functions
    private Stmt.Function Function(string kind)
    {
        // Get function name
        Token name = Consume(TokenType.IDENTIFIER, $"Expected {kind} name.");
        
        // Check for opening parenthesis for parameters
        Consume(TokenType.LEFT_PAREN, $"Expected '(' after {kind} name.");
        
        // Initialize parameters list
        List<Token> parameters = new List<Token>();
        
        // If we have (), we skip parsing the parameters as there will be none
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                // Check maximum parameters count
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters.");
                }
                    
                // Add parameter to list
                parameters.Add(Consume(TokenType.IDENTIFIER, "Expected parameter name."));
            } while (Match(TokenType.COMMA));
        }
        // Check for closing parenthesis
        Consume(TokenType.RIGHT_PAREN, "Expected ')' after parameters.");
        
        // Check for left brace
        Consume(TokenType.LEFT_BRACE, "Expected '{' before" + kind + " body");
        
        List<Stmt> body = Block();
        return new Stmt.Function(name, parameters, body);
    }

    private List<Stmt> Block()
    {
        List<Stmt> statements = new List<Stmt>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }
        
        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private Expr Expression()
    {
        return Assignment();
    }

    private Expr Assignment()
    {
        Expr expr = Or();

        if (Match(TokenType.EQUAL))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Expr.Variable)
            {
                Token name = ((Expr.Variable)expr).Name;
                return new Expr.Assign(name, value);
            }
            // This is a trick to not parse the set expression and reuse the existing get expression
            else if (expr is Expr.Get get)
            {
                return new Expr.Set(get.Obj, get.Name, value);
            }
            
            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Or()
    {
        Expr expr = And();

        while (Match(TokenType.OR))
        {
            Token op = Previous();
            Expr right = And();
            expr = new Expr.Logical(expr, op, right);
        }
        
        return expr;
    }

    private Expr And()
    {
        Expr expr = Equality();

        while (Match(TokenType.AND))
        {
            Token op = Previous();
            Expr right = Equality();
            expr = new Expr.Logical(expr, op, right);
        }
        
        return expr;
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

        while (Match(TokenType.SLASH, TokenType.STAR, TokenType.PERCENT))
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

        return Call();
    }

    private Expr Call()
    {
        Expr expr = Primary();

        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            } 
            else if (Match(TokenType.DOT))
            {
                Token name = Consume(TokenType.IDENTIFIER, "Expected property name after '.'.");
                expr = new Expr.Get(expr, name);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    private Expr FinishCall(Expr callee)
    {
        List<Expr> arguments = new List<Expr>();
        // If we match (), don't try to parse arguments as there aren't any
        if (!Check(TokenType.RIGHT_PAREN))
        {
            // parse arguments until no new ',' is found
            do
            {
                // Limit the number of arguments that can be passed to functions
                // No real reason to do this except for convention
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }
                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }
        
        // check for closing parenthesis after arguments list
        Token paren = Consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments.");
        
        return new Expr.Call(callee, paren, arguments);
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

        if (Match(TokenType.SUPER))
        {
            Token keyword = Previous();
            Consume(TokenType.DOT, "Expect '.' after 'super'.");
            Token method = Consume(TokenType.IDENTIFIER, "Expect superclass method name.");
            return new Expr.Super(keyword, method);
        }

        if (Match(TokenType.THIS)) return new Expr.This(Previous());

        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
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