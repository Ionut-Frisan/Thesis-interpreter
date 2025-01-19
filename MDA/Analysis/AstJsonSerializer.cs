using System.Text.Json;
using System.Text.Json.Serialization;

namespace MDA;

public class AstJsonSerializer : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    public string ToJson(Expr expr)
    {
        var jsonObject = expr.Accept(this);
        return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
    }

    public string ToJson(Stmt stmt)
    {
        var jsonObject = stmt.Accept(this);
        return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
    }

    public string ToJson(ICollection<Stmt> stmts)
    {
        var jsonObject = stmts.Select(stmt => stmt.Accept(this));
        return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        return new
        {
            type = "Assign",
            name = expr.Name.Lexeme,
            value = expr.Value.Accept(this)
        };
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        return new
        {
            type = "Binary",
            op = expr.Op.Lexeme,
            left = expr.Left.Accept(this),
            right = expr.Right.Accept(this)
        };
    }

    public object VisitCallExpr(Expr.Call expr)
    {
        return new
        {
            type = "Call",
            callee = expr.Callee.Accept(this),
            arguments = expr.Arguments.Select(arg => arg.Accept(this))
        };
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return new
        {
            type = "Grouping",
            expression = expr.Expr.Accept(this)
        };
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return new
        {
            type = "Literal",
            value = expr.Value
        };
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
        return new
        {
            type = "Logical",
            op = expr.Op.Lexeme,
            left = expr.Left.Accept(this),
            right = expr.Right.Accept(this)
        };
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        return new
        {
            type = "Unary",
            op = expr.Op.Lexeme,
            right = expr.Right.Accept(this)
        };
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        return new
        {
            type = "Variable",
            name = expr.Name.Lexeme
        };
    }

    public object VisitBlockStmt(Stmt.Block stmt)
    {
        return new
        {
            type = "Block",
            statements = stmt.Statements.Select(statement => statement.Accept(this))
        };
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        return new
        {
            type = "ExpressionStatement",
            expression = stmt.Expr.Accept(this)
        };
    }

    public object VisitFunctionStmt(Stmt.Function stmt)
    {
        return new
        {
            type = "Function",
            name = stmt.Name.Lexeme,
            parameters = stmt.Parameters.Select(param => param.Lexeme),
            body = stmt.Body.Select(bodyStmt => bodyStmt.Accept(this))
        };
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
        return new
        {
            type = "If",
            condition = stmt.Condition.Accept(this),
            thenBranch = stmt.ThenBranch.Accept(this),
            elseBranch = stmt.ElseBranch?.Accept(this)
        };
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        return new
        {
            type = "Print",
            expression = stmt.Expr.Accept(this)
        };
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        return new
        {
            type = "Return",
            value = stmt.Value?.Accept(this)
        };
    }

    public object VisitBreakStmt(Stmt.Break stmt)
    {
        return new
        {
            type = "Break"
        };
    }

    public object VisitContinueStmt(Stmt.Continue stmt)
    {
        return new
        {
            type = "Continue"
        };
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        return new
        {
            type = "VariableDeclaration",
            name = stmt.Name.Lexeme,
            initializer = stmt.Initializer?.Accept(this)
        };
    }

    public object VisitWhileStmt(Stmt.While stmt)
    {
        return new
        {
            type = "While",
            condition = stmt.Condition.Accept(this),
            body = stmt.Body.Accept(this)
        };
    }

    public object VisitGetExpr(Expr.Get expr)
    {
        return new
        {
            type = "Get",
            obj = expr.Obj.Accept(this),
            name = expr.Name.Lexeme
        };
    }

    public object VisitSetExpr(Expr.Set expr)
    {
        return new
        {
            type = "Set",
            obj = expr.Obj.Accept(this),
            name = expr.Name.Lexeme,
            value = expr.Value.Accept(this)
        };
    }

    public object VisitThisExpr(Expr.This expr)
    {
        return new
        {
            type = "This"
        };
    }

    public object VisitSuperExpr(Expr.Super expr)
    {
        return new
        {
            type = "Super",
            keyword = expr.Keyword.Lexeme,
            method = expr.Method.Lexeme
        };
    }

    public object VisitClassStmt(Stmt.Class stmt)
    {
        return new
        {
            type = "Class",
            name = stmt.Name.Lexeme,
            superclass = stmt.Superclass?.Name.Lexeme,
            methods = stmt.Methods.Select(method => method.Accept(this))
        };
    }

    public object VisitListExpr(Expr.List expr)
    {
        return new
        {
            type = "List",
            elements = expr.Elements.Select(element => element.Accept(this))
        };
    }

    public object VisitListAccessExpr(Expr.ListAccess expr)
    {
        return new
        {
            type = "ListAccess",
            list = expr.List.Accept(this),
            index = expr.Index.Accept(this)
        };
    }

    public object VisitListAssignExpr(Expr.ListAssign expr)
    {
        return new
        {
            type = "ListAssign",
            list = expr.List.Accept(this),
            index = expr.Index.Accept(this),
            value = expr.Value.Accept(this)
        };
    }
}