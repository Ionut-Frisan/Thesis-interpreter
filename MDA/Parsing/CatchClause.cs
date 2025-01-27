namespace MDA;

public class CatchClause
{
    public readonly Token? Variable;
    public readonly Stmt.Block Block;
    
    public CatchClause(Token? variable, Stmt.Block block)
    {
        Variable = variable;
        Block = block;
    }
}