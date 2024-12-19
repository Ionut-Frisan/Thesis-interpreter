namespace MDA;

public abstract class Stmt {
  public interface IVisitor <T> {
    T VisitExpressionStmt(Expression stmt);
    T VisitPrintStmt(Print stmt);
    T VisitVarStmt(Var stmt);
  }

  public class Expression : Stmt {
    public Expression(Expr expr) {
      this.Expr = expr;
    }

     public override T Accept<T>(IVisitor<T> visitor) {
      return visitor.VisitExpressionStmt(this);
    }

    public Expr Expr { get; set; }
  }

  public class Print : Stmt {
    public Print(Expr expr) {
      this.Expr = expr;
    }

     public override T Accept<T>(IVisitor<T> visitor) {
      return visitor.VisitPrintStmt(this);
    }

    public Expr Expr { get; set; }
  }

  public class Var : Stmt {
    public Var(Token name, Expr? initializer) {
      this.Name = name;
      this.Initializer = initializer;
    }

     public override T Accept<T>(IVisitor<T> visitor) {
      return visitor.VisitVarStmt(this);
    }

    public Token Name { get; set; }
    public Expr? Initializer { get; set; }
  }


  public abstract T Accept<T>(IVisitor<T> visitor);
}
