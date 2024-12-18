namespace MDA;

public abstract class Expr {
  public interface IVisitor <T> {
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitUnaryExpr(Unary expr);
  }

  public class Binary : Expr {
    public Binary(Expr left, Token op, Expr right) {
      this.Left = left;
      this.Op = op;
      this.Right = right;
    }

     public override T accept<T>(IVisitor<T> visitor) {
      return visitor.VisitBinaryExpr(this);
    }

    public Expr Left { get; set; }
    public Token Op { get; set; }
    public Expr Right { get; set; }
  }

  public class Grouping : Expr {
    public Grouping(Expr expression) {
      this.Expression = expression;
    }

     public override T accept<T>(IVisitor<T> visitor) {
      return visitor.VisitGroupingExpr(this);
    }

    public Expr Expression { get; set; }
  }

  public class Literal : Expr {
    public Literal(object value) {
      this.Value = value;
    }

     public override T accept<T>(IVisitor<T> visitor) {
      return visitor.VisitLiteralExpr(this);
    }

    public object Value { get; set; }
  }

  public class Unary : Expr {
    public Unary(Token op, Expr right) {
      this.Op = op;
      this.Right = right;
    }

     public override T accept<T>(IVisitor<T> visitor) {
      return visitor.VisitUnaryExpr(this);
    }

    public Token Op { get; set; }
    public Expr Right { get; set; }
  }


  public abstract T accept<T>(IVisitor<T> visitor);
}
