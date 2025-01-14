using MDA.Builtins;
namespace MDA;

public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private readonly Environment _globals = new Environment();
    private Environment _environment;
    private readonly IDictionary<Expr, int> _locals = new Dictionary<Expr, int>();
    
    private class BreakException : Exception {}
    private class ContinueException : Exception {}

    public Interpreter()
    {
        _environment = _globals;
        
        InitializeNatives();
    }
    
    private void InitializeNatives()
    {
        NativeFunctionRegistry.RegisterFromType(typeof(NativeFunctions));
        NativeFunctionRegistry.RegisterClass<RandomClass>();
        NativeFunctionRegistry.RegisterClass<ConsoleClass>();

        foreach (var name in NativeFunctionRegistry.GetAllFunctionNames())
        {
            var (func, arity) = NativeFunctionRegistry.GetFunction(name);
            _globals.Define(name, new NativeCallable(func, arity));
        }
    }
    
    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Mda.RuntimeError(error);
        }
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
        object left = Evaluate(expr.Left);

        if (expr.Op.Type == TokenType.OR)
        {
            // true || x -> don't evaluate x as expression is truthy anyway
            if (Utils.IsTruthy(left)) return left;
        }
        else
        {   
            // false && x -> don't evaluate x as expression is falsy anyway
            if (!Utils.IsTruthy(left)) return left;
        }
        
        return Evaluate(expr.Right);
    }
    
    public object VisitSetExpr(Expr.Set expr)
    {
        object obj = Evaluate(expr.Obj);
        
        if (!(obj is MdaInstance))
        {
            throw new RuntimeError(expr.Name, "Only instances have fields.");
        }

        object value = Evaluate(expr.Value);
        ((MdaInstance)obj).Set(expr.Name, value);
        return value;
    }
    
    public object VisitSuperExpr(Expr.Super expr)
    {
        int distance = _locals[expr];
        MdaClass superclass = (MdaClass)_environment.GetAt(distance, "super");
        MdaInstance obj = (MdaInstance)_environment.GetAt(distance - 1, "this");
        MdaFunction method = superclass.FindMethod(expr.Method.Lexeme);

        if (method == null)
        {
            throw new RuntimeError(expr.Method, $"Undefined property '{expr.Method.Lexeme}'.");
        }

        return method.Bind(obj);
    }
    
    public object VisitThisExpr(Expr.This expr)
    {
        return LookupVariable(expr.Keyword, expr);
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expr);
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.BANG:
                return !Utils.IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Op, right);
                return -(double)right;
        }

        // Unreachable;
        return null;
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.GREATER:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left <= (double)right;
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.MINUS:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                if (left is double && right is double)
                {
                    return (double)left + (double)right;
                }

                if (left is string && right is string)
                {
                    return (string)left + (string)right;
                }

                // Support concacatenation of strings and numbers
                if (left is string && right is double || left is double && right is string)
                {
                    return Utils.Stringify(left) + Utils.Stringify(right);
                }

                throw new RuntimeError(expr.Op, "Operands must be two strings or two numbers or a combination of strings and numbers.");
            case TokenType.SLASH:
                CheckNumberOperands(expr.Op, left, right);
                if ((double)right == 0)
                {
                    throw new RuntimeError(expr.Op, "Cannot divide by zero.");
                }

                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left * (double)right;
            case TokenType.PERCENT:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left % (double)right;
        }

        // Unreachable.
        return null;
    }

    public object? VisitCallExpr(Expr.Call expr)
    {
        // We evaluate the expression for the callee.
        // Typically, this expression is just an identifier that looks up the function by its name, but it could be anything
        object callee = Evaluate(expr.Callee);
        
        // We evaluate each of the argument expressions in order and store the resulting values in a list
        List<object> arguments = new List<object>();
        foreach (var argument in expr.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }

        if (!(callee is IMdaCallable))
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }
        
        IMdaCallable function = (IMdaCallable)callee;

        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments, but got {arguments.Count}.");
        }
        
        return function.Call(this, arguments);
    }
    
    public object VisitGetExpr(Expr.Get expr)
    {
        object obj = Evaluate(expr.Obj);
        
        // If object is a MdaList, check for the requested method
        if (obj is MdaList list)
        {
            string propertyName = expr.Name.Lexeme;
            IMdaCallable? method = list.FindMethod(propertyName);
            
            if (method != null)
            {
                return method;
            }
            
            throw new RuntimeError(expr.Name, $"Undefined method '{propertyName}' on list.");
        }
        
        if (obj is MdaInstance instance)
        {
            return instance.Get(expr.Name);
        }

        throw new RuntimeError(expr.Name, "Only instances have properties.");
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        if (Utils.IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }

        return null;
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? VisitFunctionStmt(Stmt.Function stmt)
    {
        MdaFunction function = new MdaFunction(stmt, _environment);
        _environment.Define(stmt.Name.Lexeme, function);
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        object value = Evaluate(stmt.Expr);
        Console.WriteLine(Utils.Stringify(value));
        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        object? value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }
        
        _environment.Define(stmt.Name.Lexeme, value);
        return null;
    }

    public object? VisitWhileStmt(Stmt.While stmt)
    {
        try
        {
            while (Utils.IsTruthy(Evaluate(stmt.Condition)))
            {
                try
                {
                    Execute(stmt.Body);
                }
                catch (ContinueException e)
                {
                    // continue loop
                    if (stmt.Increment != null && stmt.Body is Stmt.Block block && block.Statements.Last() is Stmt.Expression incrementStmt)
                    {
                        // we add the increment statement to a new block so it matches the depth from the resolver
                        // need to properly resolve the increment statement individually
                        Execute(new Stmt.Block(new List<Stmt>{ incrementStmt }));
                    }
                }
            }

        }
        catch (BreakException e)
        {
            // break out of loop
        }
        
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        object? value = null;
        if (stmt.Value != null) value = Evaluate(stmt.Value);

        throw new Return(value);
    }

    public object? VisitBreakStmt(Stmt.Break stmt)
    {
        throw new BreakException();
    }
    
    public object? VisitContinueStmt(Stmt.Continue stmt) {
        throw new ContinueException();
    }

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
        return null;
    }

    public object? VisitClassStmt(Stmt.Class stmt)
    {
        object superclass = null;
        if (stmt.Superclass != null)
        {
            superclass = Evaluate(stmt.Superclass);
            if (!(superclass is MdaClass))
            {
                throw new RuntimeError(stmt.Superclass.Name, "Superclass must be a class.");
            }
        }
        
        _environment.Define(stmt.Name.Lexeme, null);

        if (stmt.Superclass != null)
        {
            _environment = new Environment(_environment);
            _environment.Define("super", superclass);
        }
        
        IDictionary<string, MdaFunction> methods = new Dictionary<string, MdaFunction>();
        foreach (Stmt.Function method in stmt.Methods)
        {
            MdaFunction function = new MdaFunction(method, _environment, method.Name.Lexeme.Equals("init"));
            methods[method.Name.Lexeme] = function;
        }
        
        MdaClass klass = new MdaClass(stmt.Name.Lexeme , (MdaClass?)superclass, methods);

        if (superclass != null)
        {
            _environment = _environment.Enclosing!;
        }
        
        _environment.Assign(stmt.Name, klass);
        return null;
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        object value = Evaluate(expr.Value);
        
        if (_locals.TryGetValue(expr, out int distance))
        {
            _environment.AssignAt(distance, expr.Name, value);
        }
        else
        {
            _globals.Assign(expr.Name, value);
        }
        
        return value;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        return LookupVariable(expr.Name, expr);
    }

    public object VisitListExpr(Expr.List expr)
    {
        MdaList list = new MdaList();
        foreach (var element in expr.Elements)
        {
            list.Push(Evaluate(element));
        }

        return list;
    }

    public object VisitListAccessExpr(Expr.ListAccess expr)
    {
        object list = Evaluate(expr.List);
        if (!(list is MdaList))
        {
            throw new RuntimeError(expr.Bracket, "Only lists have indexes.");
        }
        
        object index = Evaluate(expr.Index);
        if (!(index is double))
        {
            throw new RuntimeError(expr.Bracket, "Index must be a number.");
        }
        
        int idx = (int)(double)index;
        if ((double)index != idx)
        {
            throw new RuntimeError(expr.Bracket, "Index must be an integer.");
        }

        return ((MdaList)list).Get(idx);
    }

    public object VisitListAssignExpr(Expr.ListAssign expr)
    {
        object list = Evaluate(expr.List);
        if (!(list is MdaList))
        {
            throw new RuntimeError(expr.Bracket, "Only lists have indexes.");
        }
        
        object index = Evaluate(expr.Index);
        if (!(index is double))
        {
            throw new RuntimeError(expr.Bracket, "Index must be a number.");
        }
        
        int idx = (int)(double)index;
        if ((double)index != idx)
        {
            throw new RuntimeError(expr.Bracket, "Index must be an integer.");
        }
        
        object value = Evaluate(expr.Value);
        ((MdaList)list).Set(idx, value);
        return value;
    }
    
    private object LookupVariable(Token name, Expr expr)
    {
        if (NativeFunctionRegistry.HasFunction(name.Lexeme))
        {
            var (func, arity) = NativeFunctionRegistry.GetFunction(name.Lexeme);
            return new NativeCallable(func, arity);
        }
        
        if (_locals.TryGetValue(expr, out int distance))
        {
            return _environment.GetAt(distance, name.Lexeme);
        }
        return _globals.Get(name);
    }

    private void CheckNumberOperand(Token op, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(op, "Operand must be a number");
    }

    private void CheckNumberOperands(Token op, object left, object right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(op, "Operands must be numbers");
    }

    private bool IsEqual(object? left, object? right)
    {
        if (left == null && right == null) return true;
        if (left == null || right == null) return false;

        return left.Equals(right);
    }

    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    public void Resolve(Expr expr, int depth)
    {
        _locals[expr] = depth;
    }

    public void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = _environment;
        try
        {
            _environment = environment;
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }
}