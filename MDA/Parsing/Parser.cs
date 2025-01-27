using MDA.Errors;

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

    /*
     * declaration    → classDecl
     *                  | funDecl
     *                  | varDecl
     *                  | statement ;
     */
    private Stmt? Declaration()
    {
        try
        {
            if (Match(TokenType.CLASS)) return ClassDeclaration();
            if (Match(TokenType.FUN)) return Function("function");
            if (Match(TokenType.VAR)) return VarDeclaration();

            return Statement();
        }
        catch (ParseError parseError)
        {
            Syncronize();
            return null;
        }
    }

    /*
     * classDeclaration → "class" IDENTIFIER ( "<" IDENTIFIER )?
     *                    "{" function* "}" ;
     */
    private Stmt ClassDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "PS016");

        Expr.Variable superclass = null;
        if (Match(TokenType.LESS))
        {
            Consume(TokenType.IDENTIFIER, "PS017");
            superclass = new Expr.Variable(Previous());
        }

        Consume(TokenType.LEFT_BRACE, "PS018");

        List<Stmt.Function> methods = new List<Stmt.Function>();
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            methods.Add(Function("method"));
        }

        Consume(TokenType.RIGHT_BRACE, "PS019");

        return new Stmt.Class(name, superclass, methods);
    }
    
    /*
     * statement      → exprStmt
     *                  | forStmt
     *                  | ifStmt
     *                  | printStmt
     *                  | returnStmt
     *                  | whileStmt
     *                  | breakStmt
     *                  | continueStmt
     *                  | block ;
     */
    private Stmt Statement()
    {
        if (Match((TokenType.FOR))) return ForStatement();
        if (Match(TokenType.IF)) return IfStatement();
        if (Match(TokenType.PRINT)) return PrintStatement();
        if (Match(TokenType.RETURN)) return ReturnStatement();
        if (Match(TokenType.CONTINUE)) return ContinueStatement();
        if (Match(TokenType.BREAK)) return BreakStatement();
        if (Match(TokenType.WHILE)) return WhileStatement();
        if (Match(TokenType.TRY)) return TryStatement();
        if (Match(TokenType.THROW)) return ThrowStatement();
        if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

        return ExpressionStatement();
    }

    /*
     * forStmt        → "for" "(" ( varDecl | exprStmt | ";" )
     *                expression? ";"
     *                expression? ")" statement ;
     */
    private Stmt ForStatement()
    {
        Consume(TokenType.LEFT_PAREN, "PS020");

        Stmt? initializer;
        if (Match(TokenType.SEMICOLON))
        {
            // Initializer can be omitted.
            // for( ; <condition> ; <increment>)
            initializer = null;
        }
        else if (Match(TokenType.VAR))
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

        Consume(TokenType.SEMICOLON, "PS021");

        Expr? increment = null;
        if (!Check(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }

        Consume(TokenType.RIGHT_PAREN, "PS022");

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
        body = new Stmt.While(condition, body, increment);

        if (initializer != null)
        {
            body = new Stmt.Block(new List<Stmt> { initializer, body });
        }

        return body;
    }

    /*
     * varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
     */
    private Stmt VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "PS023");

        Expr? initilizer = null;
        if (Match(TokenType.EQUAL))
        {
            initilizer = Expression();
        }

        Consume(TokenType.SEMICOLON, "PS024");
        return new Stmt.Var(name, initilizer);
    }

    /*
     * whileStmt      → "while" "(" expression ")" statement ;
     */
    private Stmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "PS025");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "PS026");
        Stmt body = Statement();

        return new Stmt.While(condition, body, null);
    }

    /*
     *ifStmt         → "if" "(" expression ")" statement
     *               ( "else" statement )? ;
     */
    private Stmt IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, "PS027");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "PS028");

        Stmt thenBranch = Statement();
        Stmt? elseBranch = null;
        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }

        return new Stmt.If(condition, thenBranch, elseBranch);
    }

    /*
     * printStmt      → "print" expression ";" ;
     */
    private Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "PS029");
        return new Stmt.Print(value);
    }

    /*
     * returnStmt     -> "return" expression? ";" ;
     */
    private Stmt ReturnStatement()
    {
        Token keyword = Previous();
        Expr? value = null;
        if (!Check(TokenType.SEMICOLON))
        {
            value = Expression();
        }

        Consume(TokenType.SEMICOLON, "PS030");
        return new Stmt.Return(keyword, value);
    }

    /*
     * continueStmt   → "continue" ";" ;
     */
    private Stmt ContinueStatement()
    {
        Consume(TokenType.SEMICOLON, "PS031");
        return new Stmt.Continue(Previous());
    }

    /*
     * breakStmt      → "break" ";" ;
     */
    private Stmt BreakStatement()
    {
        Consume(TokenType.SEMICOLON, "PS032");
        return new Stmt.Break(Previous());
    }

    /*
     * exprStmt       → expression ";" ;
     */
    private Stmt ExpressionStatement()
    {
        Expr expression = Expression();
        Consume(TokenType.SEMICOLON, "PS033");
        return new Stmt.Expression(expression);
    }

    /*
     * function       → IDENTIFIER "(" parameters? ")" block ;
     */
    private Stmt.Function Function(string kind)
    {
        // Get function name
        Token name = Consume(TokenType.IDENTIFIER, ErrorResolver.Resolve("PS034", new Dictionary<string, string> { { "kind", kind } }));

        // Check for opening parenthesis for parameters
        Consume(TokenType.LEFT_PAREN, ErrorResolver.Resolve("PS035", new Dictionary<string, string> { { "kind", kind } }));

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
                    Error(Peek(), "PS001");
                }

                // Add parameter to list
                parameters.Add(Consume(TokenType.IDENTIFIER, "PS003"));
            } while (Match(TokenType.COMMA));
        }

        // Check for closing parenthesis
        Consume(TokenType.RIGHT_PAREN, "PS004");

        // Check for left brace
        Consume(TokenType.LEFT_BRACE, ErrorResolver.Resolve("PS005", new Dictionary<string, string> { { "kind", kind } }));

        List<Stmt> body = Block();
        return new Stmt.Function(name, parameters, body);
    }

    /*
     * block          → "{" declaration* "}" ;
     */
    private List<Stmt> Block()
    {
        List<Stmt> statements = new List<Stmt>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RIGHT_BRACE, "PS006");
        return statements;
    }
    
    /*
     * expression     → assignment ;
     */
    private Expr Expression()
    {
        return Assignment();
    }

    
    /*
     * assignment     → ( call "." )? IDENTIFIER ( "=" assignment
     *                              | "+=" assignment
     *                              | "-=" assignment
     *                              | "*=" assignment
     *                              | "/=" assignment
     *                              | "%=" assignment )
     *                    | logic_or ;
     */
    private Expr Assignment()
    {
        Expr expr = Or();

        if (Match(TokenType.EQUAL))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Expr.Variable variable)
            {
                return new Expr.Assign(variable.Name, value);
            }
            if (expr is Expr.Get get)
            {
                return new Expr.Set(get.Obj, get.Name, value);
            }

            Error(equals, "PS007");
        }

        if (Match(TokenType.PLUS_EQUAL, TokenType.MINUS_EQUAL, TokenType.STAR_EQUAL,
                TokenType.SLASH_EQUAL, TokenType.PERCENT_EQUAL))
        {
            Token operatorToken = Previous();
            Expr value = Assignment();
            TokenType compoundType = operatorToken.Type;

            Expr? desugaredExpr = expr switch
            {
                Expr.Variable variable => new Expr.Binary(
                    new Expr.Variable(variable.Name),
                    GetEquivalentOperator(compoundType, operatorToken),
                    value
                ),
                Expr.Get get => new Expr.Binary(
                    new Expr.Get(get.Obj, get.Name),
                    GetEquivalentOperator(compoundType, operatorToken),
                    value
                ),
                _ => null
            };
            
            if (desugaredExpr == null)
            {
                Mda.Error(operatorToken, "PS008");
            }

            return expr switch
            {
                Expr.Variable variable => new Expr.Assign(variable.Name, desugaredExpr),
                Expr.Get get => new Expr.Set(get.Obj, get.Name, desugaredExpr),
                _ => expr // Unreachable, but required to satisfy the compiler
            };
        }

        if (Match(TokenType.PLUS_PLUS, TokenType.MINUS_MINUS))
        {
            Token operatorToken = Previous();
            TokenType incrementType = operatorToken.Type;

            Expr? desugaredExpr = expr switch
            {
                Expr.Variable variable => new Expr.Binary(
                    new Expr.Variable(variable.Name),
                    incrementType == TokenType.PLUS_PLUS
                        ? new Token(TokenType.PLUS, "+", null, operatorToken.Line, operatorToken.Column)
                        : new Token(TokenType.MINUS, "-", null, operatorToken.Line, operatorToken.Column),
                    new Expr.Literal(1.0)
                ),
                Expr.Get get => new Expr.Binary(
                    new Expr.Get(get.Obj, get.Name),
                    incrementType == TokenType.PLUS_PLUS
                        ? new Token(TokenType.PLUS, "+", null, operatorToken.Line, operatorToken.Column)
                        : new Token(TokenType.MINUS, "-", null, operatorToken.Line, operatorToken.Column),
                    new Expr.Literal(1.0)
                ),
                _ => null
            };
            
            if (desugaredExpr == null)
            {
                Mda.Error(operatorToken, "PS009");
            }

            return expr switch
            {
                Expr.Variable variable => new Expr.Assign(variable.Name, desugaredExpr),
                Expr.Get get => new Expr.Set(get.Obj, get.Name, desugaredExpr),
                _ => expr // Unreachable, but required to satisfy the compiler
            };
        }

        return expr;
    }

    /*
     * logic_or       → logic_and ( "or" logic_and )* ;
     */
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

    /*
     * logic_and      → equality ( "and" equality )* ;
     */
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

    /*
     * call           → primary ( "(" arguments? ")" | "." IDENTIFIER )* ;
     */
    private Expr Call()
    {
        Expr expr = Primary();

        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }
            else if (Match(TokenType.LEFT_BRACKET))
            {
                expr = FinishArrayAccess(expr);
            }
            else if (Match(TokenType.DOT))
            {
                Token name = Consume(TokenType.IDENTIFIER, "PS010");
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
                    Error(Peek(), "PS002");
                }

                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }

        // check for closing parenthesis after arguments list
        Token paren = Consume(TokenType.RIGHT_PAREN, "PS011");

        return new Expr.Call(callee, paren, arguments);
    }

    public Expr FinishArrayAccess(Expr list)
    {
        Token bracket = Previous();
        Expr index = Expression();
        Consume(TokenType.RIGHT_BRACKET, "PS038");

        if (Match(TokenType.EQUAL))
        {
            Expr value = Expression();
            return new Expr.ListAssign(list, index, value, bracket);
        }

        return new Expr.ListAccess(list, index, bracket);
    }

    public Stmt TryStatement()
    {
        Consume(TokenType.LEFT_BRACE, "PS039");
        Stmt.Block tryBlock = new Stmt.Block(Block());
        CatchClause? catchClause = null;
        if (Match(TokenType.CATCH))
        {
            Consume(TokenType.LEFT_PAREN, "PS040");
            Token? variable = null;
            if (!Match(TokenType.RIGHT_PAREN))
            {
                variable = Consume(TokenType.IDENTIFIER, "PS041");   
            }

            Consume(TokenType.RIGHT_PAREN, "PS042");
            Consume(TokenType.LEFT_BRACE, "PS043");
            List<Stmt> catchBlock = Block();
            catchClause = new CatchClause(variable, new Stmt.Block(catchBlock));
        }
        
        Stmt.Block? finallyBlock = null;
        if (Match(TokenType.FINALLY))
        {
            Consume(TokenType.LEFT_BRACE, "PS044");
            finallyBlock = new(Block());
        }

        return new Stmt.Try(tryBlock, catchClause, finallyBlock);
    }

    public Stmt ThrowStatement()
    {
        Token keyword = Previous();
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "PS045");
        return new Stmt.Throw(keyword, value);
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
            Consume(TokenType.DOT, "PS012");
            Token method = Consume(TokenType.IDENTIFIER, "PS013");
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
            Consume(TokenType.RIGHT_PAREN, "PS014");
            return new Expr.Grouping(expr);
        }

        if (Match(TokenType.LEFT_BRACKET))
        {
            return listLiteral();
        }

        throw Error(Peek(), "PS015");
    }
    
    private Expr listLiteral()
    {
        List<Expr> elements = new List<Expr>();
        if (!Check(TokenType.RIGHT_BRACKET))
        {
            do
            {
                if (elements.Count >= 255)
                {
                    Error(Peek(), "PS036");
                }

                elements.Add(Expression());
            } while (Match(TokenType.COMMA));
        }
            
        Consume(TokenType.RIGHT_BRACKET, "PS037");
        return new Expr.List(elements);
    }

    /*
     * Consume the current token if it is of the given type.
     * If it is not, report a ParseError.
     */
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
        string formattedMessage = ErrorResolver.Resolve(message);
        Mda.Error(token, formattedMessage);
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

    /*
     * Returns the equivalent operator token for a compound assignment operator.
     */
    private Token GetEquivalentOperator(TokenType compoundType, Token token)
    {
        return compoundType switch
        {
            TokenType.PLUS_EQUAL => new Token(TokenType.PLUS, "+", null, token.Line, token.Column),
            TokenType.MINUS_EQUAL => new Token(TokenType.MINUS, "-", null, token.Line, token.Column),
            TokenType.STAR_EQUAL => new Token(TokenType.STAR, "*", null, token.Line, token.Column),
            TokenType.SLASH_EQUAL => new Token(TokenType.SLASH, "/", null, token.Line, token.Column),
            TokenType.PERCENT_EQUAL => new Token(TokenType.PERCENT, "%", null, token.Line, token.Column),
            _ => throw new ArgumentException("Unknown compound assignment type."),
        };
    }
}