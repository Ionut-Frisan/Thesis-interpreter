 /* 
  This file is automatically generated. 
*/

namespace MDA;

public abstract class Stmt {
    public interface IVisitor <T> {
        T VisitBlockStmt(Block stmt);
        T VisitClassStmt(Class stmt);
        T VisitExpressionStmt(Expression stmt);
        T VisitFunctionStmt(Function stmt);
        T VisitIfStmt(If stmt);
        T VisitPrintStmt(Print stmt);
        T VisitReturnStmt(Return stmt);
        T VisitBreakStmt(Break stmt);
        T VisitContinueStmt(Continue stmt);
        T VisitVarStmt(Var stmt);
        T VisitWhileStmt(While stmt);
        T VisitThrowStmt(Throw stmt);
        T VisitTryStmt(Try stmt);
    }

    public class Block : Stmt {
        public Block(List<Stmt> statements) {
            this.Statements = statements;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitBlockStmt(this);
        }

        public List<Stmt> Statements { get; set; }
    }

    public class Class : Stmt {
        public Class(Token name, Expr.Variable? superclass, List<Stmt.Function> methods) {
            this.Name = name;
            this.Superclass = superclass;
            this.Methods = methods;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitClassStmt(this);
        }

        public Token Name { get; set; }
        public Expr.Variable? Superclass { get; set; }
        public List<Stmt.Function> Methods { get; set; }
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

    public class Function : Stmt {
        public Function(Token name, List<Token> parameters, List<Stmt> body) {
            this.Name = name;
            this.Parameters = parameters;
            this.Body = body;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitFunctionStmt(this);
        }

        public Token Name { get; set; }
        public List<Token> Parameters { get; set; }
        public List<Stmt> Body { get; set; }
    }

    public class If : Stmt {
        public If(Expr condition, Stmt thenBranch, Stmt? elseBranch) {
            this.Condition = condition;
            this.ThenBranch = thenBranch;
            this.ElseBranch = elseBranch;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitIfStmt(this);
        }

        public Expr Condition { get; set; }
        public Stmt ThenBranch { get; set; }
        public Stmt? ElseBranch { get; set; }
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

    public class Return : Stmt {
        public Return(Token keyword, Expr? value) {
            this.Keyword = keyword;
            this.Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitReturnStmt(this);
        }

        public Token Keyword { get; set; }
        public Expr? Value { get; set; }
    }

    public class Break : Stmt {
        public Break(Token keyword) {
            this.Keyword = keyword;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitBreakStmt(this);
        }

        public Token Keyword { get; set; }
    }

    public class Continue : Stmt {
        public Continue(Token keyword) {
            this.Keyword = keyword;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitContinueStmt(this);
        }

        public Token Keyword { get; set; }
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

    public class While : Stmt {
        public While(Expr condition, Stmt body, Expr? increment) {
            this.Condition = condition;
            this.Body = body;
            this.Increment = increment;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitWhileStmt(this);
        }

        public Expr Condition { get; set; }
        public Stmt Body { get; set; }
        public Expr? Increment { get; set; }
    }

    public class Throw : Stmt {
        public Throw(Token keyword, Expr value) {
            this.Keyword = keyword;
            this.Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitThrowStmt(this);
        }

        public Token Keyword { get; set; }
        public Expr Value { get; set; }
    }

    public class Try : Stmt {
        public Try(Stmt.Block tryBlock, CatchClause? catchClause, Stmt.Block? finallyBlock) {
            this.TryBlock = tryBlock;
            this.CatchClause = catchClause;
            this.FinallyBlock = finallyBlock;
        }

        public override T Accept<T>(IVisitor<T> visitor) {
            return visitor.VisitTryStmt(this);
        }

        public Stmt.Block TryBlock { get; set; }
        public CatchClause? CatchClause { get; set; }
        public Stmt.Block? FinallyBlock { get; set; }
    }


  public abstract T Accept<T>(IVisitor<T> visitor);
}
