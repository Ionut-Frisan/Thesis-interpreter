using System.Text;

namespace MDA;

public class AstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
{
    public AstPrinter()
    {
    }

    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string Print(Stmt stmt)
    {
        return stmt.Accept(this);
    }

    public string VisitAssignExpr(Expr.Assign expr)
    {
        return Parenthesize("assign", expr.Name.Lexeme, expr.Value);
    }

    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string VisitCallExpr(Expr.Call expr)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("(call ").Append(expr.Callee.Accept(this));
        foreach (var arg in expr.Arguments)
        {
            builder.Append(" ").Append(arg.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expr);
    }

    public string VisitLiteralExpr(Expr.Literal expr)
    {
        if (expr.Value == null) return "null";
        return expr.Value.ToString();
    }

    public string VisitLogicalExpr(Expr.Logical expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string VisitVariableExpr(Expr.Variable expr)
    {
        return expr.Name.Lexeme;
    }

    public string VisitBlockStmt(Stmt.Block stmt)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("{");
        foreach (var statement in stmt.Statements)
        {
            builder.Append(" ").Append(statement.Accept(this));
        }
        builder.Append("}");
        return builder.ToString();
    }

    public string VisitExpressionStmt(Stmt.Expression stmt)
    {
        return Parenthesize(";", stmt.Expr);
    }

    public string VisitFunctionStmt(Stmt.Function stmt)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("(fun ").Append(stmt.Name.Lexeme).Append("(");
        foreach (var param in stmt.Parameters)
        {
            if (param != stmt.Parameters[0]) builder.Append(" ");
            builder.Append(param.Lexeme);
        }
        builder.Append(") ");
        foreach (var bodyStmt in stmt.Body)
        {
            builder.Append(bodyStmt.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }

    public string VisitIfStmt(Stmt.If stmt)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("(if ").Append(stmt.Condition.Accept(this)).Append(" ")
            .Append(stmt.ThenBranch.Accept(this));
        if (stmt.ElseBranch != null)
        {
            builder.Append(" else ").Append(stmt.ElseBranch.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }

    public string VisitPrintStmt(Stmt.Print stmt)
    {
        return Parenthesize("print", stmt.Expr);
    }

    public string VisitReturnStmt(Stmt.Return stmt)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("(return");
        if (stmt.Value != null)
        {
            builder.Append(" ").Append(stmt.Value.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }

    public string VisitVarStmt(Stmt.Var stmt)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("(var ").Append(stmt.Name.Lexeme);
        if (stmt.Initializer != null)
        {
            builder.Append(" = ").Append(stmt.Initializer.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }

    public string VisitWhileStmt(Stmt.While stmt)
    {
        return Parenthesize("while", stmt.Condition, stmt.Body);
    }
    
    public string VisitGetExpr(Expr.Get expr)
    {
        return Parenthesize("get", expr.Obj, expr.Name.Lexeme);
    }

    public string VisitSetExpr(Expr.Set expr)
    {
        return Parenthesize("set", expr.Obj, expr.Name.Lexeme, expr.Value);
    }

    public string VisitThisExpr(Expr.This expr)
    {
        return "this";
    }

    public string VisitSuperExpr(Expr.Super expr)
    {
        return Parenthesize("super", expr.Keyword.Lexeme, expr.Method.Lexeme);
    }
    
    public string VisitClassStmt(Stmt.Class stmt)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("(class ").Append(stmt.Name.Lexeme);
        if (stmt.Superclass != null)
        {
            builder.Append(" < ").Append(stmt.Superclass.Name.Lexeme);
        }
        builder.Append(" ");
        foreach (var method in stmt.Methods)
        {
            builder.Append(method.Accept(this));
        }
        builder.Append(")");
        return builder.ToString();
    }

    private string Parenthesize(string name, params object[] parts)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("(").Append(name);
        foreach (var part in parts)
        {
            builder.Append(" ");
            if (part is Expr)
            {
                builder.Append(((Expr)part).Accept(this));
            }
            else if (part is Stmt)
            {
                builder.Append(((Stmt)part).Accept(this));
            }
            else
            {
                builder.Append(part);
            }
        }
        builder.Append(")");
        return builder.ToString();
    }
}