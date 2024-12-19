using System.Data;
using Microsoft.VisualBasic.CompilerServices;

namespace MDA;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private Environment environment = new Environment();
    
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
            // true || x -> dont evaluate x as expression is truthy anyway
            if (IsTruthy(left)) return left;
        }
        else
        {   
            // false && x -> dont evaluate x as expression is falsy anyway
            if (IsTruthy(left)) return left;
        }
        
        return Evaluate(expr.Right);
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expr);
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Op, right);
                return -(double)right;
        }

        // Unreachable;
        return null;
    }

    public object VisitBinaryExpr(Expr.Binary expr)
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

                throw new RuntimeError(expr.Op, "Operands must be two strings or two numbers.");
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
        }

        // Unreachable.
        return null;
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }

        return null;
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        object value = Evaluate(stmt.Expr);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }
        
        environment.Define(stmt.Name.Lexeme, value);
        return null;
    }

    public object VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(environment));
        return null;
    }

    public object VisitAssignExpr(Expr.Assign stmt)
    {
        object value = Evaluate(stmt.Value);
        environment.Assign(stmt.Name, value);
        return value;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        return environment.Get(expr.Name);
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

    private bool IsTruthy(object value)
    {
        if (value == null) return false;
        if (value is bool) return (bool)value;
        if (value is double) return (double)value != 0;
        if (value is string) return (string)value != "";
        return true;
    }

    private bool IsEqual(object left, object right)
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

    private void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = this.environment;
        try
        {
            this.environment = environment;
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            this.environment = previous;
        }
    }

    private string Stringify(object value)
    {
        if (value == null) return "null";
        if (value is double)
        {
            string text = ((double)value).ToString();

            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return value.ToString();
    }
}