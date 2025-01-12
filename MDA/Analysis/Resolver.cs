using MDA.Errors;

namespace MDA;

public class Resolver : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private readonly Interpreter _interpreter;
    private readonly Stack<IDictionary<string, bool>> _scopes = new Stack<IDictionary<string, bool>>();
    private FunctionType _currentFunction = FunctionType.NONE;
    private int _loopDepth = 0;

    private enum ClassType
    {
        NONE,
        CLASS,
        SUBCLASS,
    }
    
    private ClassType _currentClass = ClassType.NONE;
    
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

    public object? VisitClassStmt(Stmt.Class stmt)
    {
        ClassType enclosingClass = _currentClass;
        _currentClass = ClassType.CLASS;
        
        Declare(stmt.Name);
        Define(stmt.Name);

        if (stmt.Superclass != null && stmt.Name.Lexeme.Equals(stmt.Superclass.Name.Lexeme))
        {
            Mda.Error(stmt.Superclass.Name, ErrorResolver.Resolve("RS001"));
        }

        if (stmt.Superclass != null)
        {
            _currentClass = ClassType.SUBCLASS;
            Resolve(stmt.Superclass);
        }
        
        if (stmt.Superclass != null)
        {
            BeginScope();
            _scopes.Peek()["super"] = true;
        }
        
        BeginScope();
        _scopes.Peek()["this"] = true;
        
        foreach(Stmt.Function method in stmt.Methods)
        {
            FunctionType declaration = FunctionType.METHOD;
            if (method.Name.Lexeme.Equals("init"))
            {
                declaration = FunctionType.INITIALIZER;
            }
            
            ResolveFunction(method, declaration);
        }
        
        EndScope();
        
        if (stmt.Superclass != null) EndScope();
        
        _currentClass = enclosingClass;
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
            Mda.Error(stmt.Keyword, ErrorResolver.Resolve("RS002"));
        }
        
        if (stmt.Value != null)
        {
            if (_currentFunction == FunctionType.INITIALIZER)
            {
                Mda.Error(stmt.Keyword, ErrorResolver.Resolve("RS003"));
            }
            Resolve(stmt.Value);
        }
        return null;
    }

    public object? VisitBreakStmt(Stmt.Break stmt)
    {
        if (_loopDepth == 0)
        {
            Mda.Error(stmt.Keyword, ErrorResolver.Resolve("RS004"));
        }
        return null;
    }    
    
    public object? VisitContinueStmt(Stmt.Continue stmt)
    {
        if (_loopDepth == 0)
        {
            Mda.Error(stmt.Keyword, ErrorResolver.Resolve("RS005"));
        }
        return null;
    }
    
    public object? VisitWhileStmt(Stmt.While stmt)
    {
        Resolve(stmt.Condition);
        
        _loopDepth++;
        Resolve(stmt.Body);
        _loopDepth--;
        
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
    
    public object? VisitGetExpr(Expr.Get expr)
    {
        Resolve(expr.Obj);
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

    public object? VisitSetExpr(Expr.Set expr)
    {
        Resolve(expr.Value);
        Resolve(expr.Obj);
        
        return null;
    }

    public object? VisitSuperExpr(Expr.Super expr)
    {
        if (_currentClass == ClassType.NONE)
        {
            Mda.Error(expr.Keyword, ErrorResolver.Resolve("RS006"));
        }
        else if (_currentClass != ClassType.SUBCLASS)
        {
            Mda.Error(expr.Keyword, ErrorResolver.Resolve("RS007"));
        }

        ResolveLocal(expr, expr.Keyword);
        return null;
    }
    
    public object? VisitThisExpr(Expr.This expr)
    {
        if (_currentClass == ClassType.NONE)
        {
            Mda.Error(expr.Keyword, ErrorResolver.Resolve("RS008"));
            return null;
        }
        
        ResolveLocal(expr, expr.Keyword);
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
                Mda.Error(expr.Name, ErrorResolver.Resolve("RS009"));
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
            Mda.Error(name, ErrorResolver.Resolve("RS010", new Dictionary<string, string> { { "name", name.Lexeme } }));
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