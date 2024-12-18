using System.Data;
using Microsoft.VisualBasic.CompilerServices;

namespace MDA;

public class Interpreter : Expr.IVisitor<object>
{

  public void Interpret(Expr expr)
  {
    try
    {
      object value = Evaluate(expr);
      Console.WriteLine(Stringify(value));
    }
    catch (RuntimeError error)
    {
      Mda.RuntimeError(error);
    }
  }
  
  public object VisitLiteralExpr(Expr.Literal expr)
  {
    return expr.Value;
  }

  public object VisitGroupingExpr(Expr.Grouping expr)
  {
    return Evaluate(expr.Expression);
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
    return expr.accept(this);
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