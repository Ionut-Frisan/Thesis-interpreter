// using System.Text;
//
// namespace MDA;
//
// public class AstPrinter : Expr.IVisitor<string>
// {
//     public AstPrinter()
//     {
//     }
//
//     public string Print(Expr expr)
//     {
//         return expr.Accept(this);
//     }
//
//     public string VisitBinaryExpr(Expr.Binary expr)
//     {
//         return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
//     }
//
//     public string VisitGroupingExpr(Expr.Grouping expr)
//     {
//         return Parenthesize("group", expr.Expr);
//     }
//
//     public string VisitLiteralExpr(Expr.Literal expr)
//     {
//         if (expr.Value is null) return "null";
//         return expr.Value.ToString();
//     }
//
//     public string VisitUnaryExpr(Expr.Unary expr)
//     {
//         return Parenthesize(expr.Op.Lexeme, expr.Right);
//     }
//
//     // params is similar to Expr... exprs
//     private string Parenthesize(string name, params Expr[] exprs)
//     {
//         StringBuilder builder = new StringBuilder();
//         builder.Append("(").Append(name);
//         foreach (var expr in exprs)
//         {
//             builder.Append(" ");
//             builder.Append(expr.Accept(this));
//         }
//
//         builder.Append(")");
//
//         return builder.ToString();
//     }
// }