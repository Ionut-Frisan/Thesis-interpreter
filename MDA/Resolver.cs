namespace MDA;

public class Resolver : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private readonly Interpreter _interpreter;
    private readonly Stack<IDictionary<string, bool>> _scopes = new Stack<IDictionary<string, bool>>();
    private FunctionType _currentFunction = FunctionType.NONE;
    
    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }
    
    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }
    
    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Resolve(stmt.Expr);
        return null;
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);
        if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch!);
        return null;
    }
    
    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        Resolve(stmt.Expr);
        return null;
    }
    
    public object? VisitReturnStmt(Stmt.Return stmt)
    {
        if (_currentFunction == FunctionType.NONE)
        {
            Mda.Error(stmt.Keyword, "Cannot return from top-level code.");
        }
        
        if (stmt.Value != null)
        {
            Resolve(stmt.Value);
        }
        return null;
    }
    
    public object? VisitWhileStmt(Stmt.While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return null;
    }
    
    public object? VisitFunctionStmt(Stmt.Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);
        
        ResolveFunction(stmt, FunctionType.FUNCTION);
        return null;
    }
    
    public object? VisitVarStmt(Stmt.Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);
        return null;
    }
    
    public object? VisitAssignExpr(Expr.Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return null;
    }
    
    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object? VisitCallExpr(Expr.Call expr)
    {
        Resolve(expr.Callee);

        foreach (var arg in expr.Arguments)
        {
            Resolve(arg);
        }

        return null;
    }
    
    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        Resolve(expr.Expr);
        return null;
    }
    
    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return null;
    }
    
    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }
    
    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        Resolve(expr.Right);
        return null;
    }
    
    public object? VisitVariableExpr(Expr.Variable expr)
    {
        if (_scopes.Count != 0)
        {
            IDictionary<string, bool> scope = _scopes.Peek();
            if (scope.ContainsKey(expr.Name.Lexeme) && scope[expr.Name.Lexeme] == false)
            {
                Mda.Error(expr.Name, "Cannot read local variable in its own initializer.");
            }
        }
        
        ResolveLocal(expr, expr.Name);
        return null;
    }
    
    public void Resolve(List<Stmt> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }
    
    public void Resolve(Stmt stmt)
    {
        stmt.Accept(this);
    }
    
    public void Resolve(Expr expr)
    {
        expr.Accept(this);
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
        
        Resolve(function.Body);
        EndScope();
        _currentFunction = enclosingFunction;
    }
    
    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }
    
    private void EndScope()
    {
        _scopes.Pop();
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0) return;

        IDictionary<string, bool> scope = _scopes.Peek();
        if (scope.ContainsKey(name.Lexeme))
        {
            Mda.Error(name, $"Variable '{name.Lexeme}' already declared in this scope.");
        }
        
        scope[name.Lexeme] = false;
    }
    
    private void Define(Token name)
    {
        if (_scopes.Count == 0) return;

        _scopes.Peek()[name.Lexeme] = true;
    }

    private void ResolveLocal(Expr expr, Token name)
    {
        // Traverse the scopes from the innermost to the outermost scope to find the variable
        for (int i = 0; i < _scopes.Count; i++)
        {
            if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
            {
                _interpreter.Resolve(expr, i);
                return;
            }
        }
    }
}