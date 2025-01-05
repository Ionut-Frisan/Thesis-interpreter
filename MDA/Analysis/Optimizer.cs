namespace MDA;

/*
 * TODO: also optimize the following case for constant folding:
 * var a = 1 + 2;
 * var b = 1 - a + 3 * 4; // should be either -a + 13 or if we can replace a with 3, then 1 - 3 + 3 * 4 = 10
 */

public class Optimizer : Expr.IVisitor<Expr>, Stmt.IVisitor<Stmt>
{
    private readonly Stack<IDictionary<string, object?>> _scopes = new Stack<IDictionary<string, object?>>();
    private FunctionType _currentFunction = FunctionType.NONE;
    private enum ClassType
    {
        NONE,
        CLASS,
        SUBCLASS,
    }

    private ClassType _currentClass = ClassType.NONE;
    private List<Stmt> _statements;

    public Optimizer(List<Stmt> statements)
    {
        _statements = statements;
    }

    public List<Stmt> Optimize()
    {
        List<Stmt> optimizedStatements = new List<Stmt>();
        foreach (var statement in _statements)
        {
            optimizedStatements.Add(Optimize(statement));
        }
        return optimizedStatements;
    }

    public Expr VisitAssignExpr(Expr.Assign expr)
    {
        Expr value = Optimize(expr.Value);
        ResolveLocal(expr, expr.Name);
        return new Expr.Assign(expr.Name, value);
    }

    public Expr VisitBinaryExpr(Expr.Binary expr)
    {
        Expr left = Optimize(expr.Left);
        Expr right = Optimize(expr.Right);
        
        // Also optimize Expr.Variable if the variable is defined. Note: watch out for while loop increment variables and other cases
        if (left is Expr.Literal leftLiteral && right is Expr.Literal rightLiteral)
        {
            return FoldBinary(expr.Op, leftLiteral, rightLiteral);
        }

        return new Expr.Binary(left, expr.Op, right);
    }

    public Expr VisitCallExpr(Expr.Call expr)
    {
        Expr callee = Optimize(expr.Callee);
        List<Expr> arguments = expr.Arguments.Select(Optimize).ToList();
        return new Expr.Call(callee, expr.Paren, arguments);
    }

    public Expr VisitGetExpr(Expr.Get expr)
    {
        Expr obj = Optimize(expr.Obj);
        return new Expr.Get(obj, expr.Name);
    }

    public Expr VisitSetExpr(Expr.Set expr)
    {
        Expr obj = Optimize(expr.Obj);
        Expr value = Optimize(expr.Value);
        return new Expr.Set(obj, expr.Name, value);
    }

    public Expr VisitGroupingExpr(Expr.Grouping expr)
    {
        Expr optimizedExpr = Optimize(expr.Expr);
        if (optimizedExpr is Expr.Literal)
        {
            return optimizedExpr;
        }
        return new Expr.Grouping(optimizedExpr);
    }

    public Expr VisitLiteralExpr(Expr.Literal expr)
    {
        return expr;
    }

    public Expr VisitLogicalExpr(Expr.Logical expr)
    {
        Expr left = Optimize(expr.Left);
        Expr right = Optimize(expr.Right);

        if (left is Expr.Literal leftLiteral && right is Expr.Literal rightLiteral)
        {
            return FoldLogical(expr.Op, leftLiteral, rightLiteral);
        }

        return new Expr.Logical(left, expr.Op, right);
    }

    public Expr VisitUnaryExpr(Expr.Unary expr)
    {
        Expr right = Optimize(expr.Right);

        if (right is Expr.Literal rightLiteral)
        {
            return FoldUnary(expr.Op, rightLiteral);
        }

        return new Expr.Unary(expr.Op, right);
    }

    public Expr VisitVariableExpr(Expr.Variable expr)
    {
        ResolveLocal(expr, expr.Name);
        return expr;
    }

    public Expr VisitThisExpr(Expr.This expr)
    {
        ResolveLocal(expr, expr.Keyword);
        return expr;
    }

    public Expr VisitSuperExpr(Expr.Super expr)
    {
        ResolveLocal(expr, expr.Keyword);
        return expr;
    }

    public Stmt VisitBlockStmt(Stmt.Block stmt)
    {
        BeginScope();
        List<Stmt> statements = stmt.Statements.Select(Optimize).ToList();
        EndScope();
        return new Stmt.Block(statements);
    }

    public Stmt VisitClassStmt(Stmt.Class stmt)
    {
        ClassType enclosingClass = _currentClass;
        _currentClass = ClassType.CLASS;

        Declare(stmt.Name);
        Define(stmt.Name);

        if (stmt.Superclass != null)
        {
            _currentClass = ClassType.SUBCLASS;
            ResolveLocal(stmt.Superclass, stmt.Superclass.Name);
        }

        BeginScope();
        _scopes.Peek()["this"] = true;

        foreach (Stmt.Function method in stmt.Methods)
        {
            ResolveFunction(method, FunctionType.METHOD);
        }

        EndScope();

        _currentClass = enclosingClass;
        return stmt;
    }

    public Stmt VisitExpressionStmt(Stmt.Expression stmt)
    {
        Expr expr = Optimize(stmt.Expr);
        // Update the original statement list with the optimized expression
        stmt.Expr = expr;
        return stmt;
    }


    public Stmt VisitIfStmt(Stmt.If stmt)
    {
        Expr condition = Optimize(stmt.Condition);
        Stmt thenBranch = Optimize(stmt.ThenBranch);
        Stmt? elseBranch = stmt.ElseBranch != null ? Optimize(stmt.ElseBranch) : null;
        return new Stmt.If(condition, thenBranch, elseBranch);
    }

    public Stmt VisitPrintStmt(Stmt.Print stmt)
    {
        Expr expr = Optimize(stmt.Expr);
        return new Stmt.Print(expr);
    }

    public Stmt VisitReturnStmt(Stmt.Return stmt)
    {
        Expr? value = stmt.Value != null ? Optimize(stmt.Value) : null;
        return new Stmt.Return(stmt.Keyword, value);
    }

    public Stmt VisitWhileStmt(Stmt.While stmt)
    {
        Expr condition = Optimize(stmt.Condition);
        Stmt body = Optimize(stmt.Body);

        // Recursively optimize the body of the while loop
        if (body is Stmt.Block block)
        {
            block.Statements = block.Statements.Select(Optimize).ToList();
        }

        return new Stmt.While(condition, body);
    }


    public Stmt VisitFunctionStmt(Stmt.Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);

        ResolveFunction(stmt, FunctionType.FUNCTION);

        // Recursively optimize the body of the function
        stmt.Body = stmt.Body.Select(Optimize).ToList();

        return stmt;
    }


    public Stmt VisitVarStmt(Stmt.Var stmt)
    {
        Declare(stmt.Name);
        Expr? initializer = stmt.Initializer != null ? Optimize(stmt.Initializer) : null;
        Define(stmt.Name);
        return new Stmt.Var(stmt.Name, initializer);
    }

    private Expr Optimize(Expr expr)
    {
        return expr.Accept(this);
    }

    private Stmt Optimize(Stmt stmt)
    {
        return stmt.Accept(this);
    }

    private void ResolveFunction(Stmt.Function function, FunctionType type)
    {
        FunctionType enclosingFunction = _currentFunction;
        _currentFunction = type;

        BeginScope();

        foreach (var param in function.Parameters)
        {
            Declare(param);
            Define(param);
        }

        // Optimize the body of the function
        for (int i = 0; i < function.Body.Count; i++)
        {
            function.Body[i] = Optimize(function.Body[i]);
        }

        EndScope();
        _currentFunction = enclosingFunction;
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, object?>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0) return;

        IDictionary<string, object?> scope = _scopes.Peek();
        if (scope.ContainsKey(name.Lexeme))
        {
            // Variable already declared in this scope
            // We can safely ignore this case in optimization phase
            return;
        }

        scope[name.Lexeme] = null;
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0) return;

        _scopes.Peek()[name.Lexeme] = true;
    }

    private void ResolveLocal(Expr expr, Token name)
    {
        for (int i = 0; i < _scopes.Count; i++)
        {
            if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
            {
                // Assuming Interpreter.Resolve is a method that resolves the variable
                // _interpreter.Resolve(expr, i);
                return;
            }
        }
    }

    private Expr FoldBinary(Token op, Expr.Literal left, Expr.Literal right)
    {
        if (left.Value is double leftVal && right.Value is double rightVal)
        {
            return op.Type switch
            {
                TokenType.PLUS => new Expr.Literal(leftVal + rightVal),
                TokenType.MINUS => new Expr.Literal(leftVal - rightVal),
                TokenType.STAR => new Expr.Literal(leftVal * rightVal),
                TokenType.SLASH => new Expr.Literal(leftVal / rightVal),
                TokenType.PERCENT => new Expr.Literal(leftVal % rightVal),
                TokenType.GREATER => new Expr.Literal(leftVal > rightVal),
                TokenType.GREATER_EQUAL => new Expr.Literal(leftVal >= rightVal),
                TokenType.LESS => new Expr.Literal(leftVal < rightVal),
                TokenType.LESS_EQUAL => new Expr.Literal(leftVal <= rightVal),
                TokenType.EQUAL_EQUAL => new Expr.Literal(leftVal == rightVal),
                TokenType.BANG_EQUAL => new Expr.Literal(leftVal != rightVal),
                _ => new Expr.Binary(left, op, right)
            };
        }

        if (left.Value is string leftStr && right.Value is string rightStr)
        {
            return op.Type switch
            {
                TokenType.PLUS => new Expr.Literal(leftStr + rightStr),
                _ => new Expr.Binary(left, op, right)
            };
        }
        if (left.Value is string ls && right.Value is double rightDouble)
        {
            return op.Type switch
            {
                TokenType.PLUS => new Expr.Literal(ls + Utils.Stringify(rightDouble)),
                _ => new Expr.Binary(left, op, right)
            };
        }

        if (left.Value is double leftDouble && right.Value is string rs)
        {
            return op.Type switch
            {
                TokenType.PLUS => new Expr.Literal(leftDouble + Utils.Stringify(rs)),
                _ => new Expr.Binary(left, op, right)
            };
        }
        
        

        return new Expr.Binary(left, op, right);
    }

    private Expr FoldLogical(Token op, Expr.Literal left, Expr.Literal right)
    {
        if (left.Value is bool leftVal && right.Value is bool rightVal)
        {
            return op.Type switch
            {
                TokenType.AND => new Expr.Literal(leftVal && rightVal),
                TokenType.OR => new Expr.Literal(leftVal || rightVal),
                _ => new Expr.Logical(left, op, right)
            };
        }

        return new Expr.Logical(left, op, right);
    }

    private Expr FoldUnary(Token op, Expr.Literal right)
    {
        if (right.Value is double rightVal)
        {
            return op.Type switch
            {
                TokenType.MINUS => new Expr.Literal(-rightVal),
                _ => new Expr.Unary(op, right)
            };
        }

        if (right.Value is bool rightBool)
        {
            return op.Type switch
            {
                TokenType.BANG => new Expr.Literal(!rightBool),
                _ => new Expr.Unary(op, right)
            };
        }

        return new Expr.Unary(op, right);
    }
}