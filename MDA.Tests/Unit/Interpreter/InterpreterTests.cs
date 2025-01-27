namespace MDA.Tests.Unit.Interpreter;

public class InterpreterTests
{
    private class TestErrorReporter : IErrorReporter
    {
        public bool HadError { get; private set; }
        public bool HadRuntimeError { get; private set; }
        public string? LastErrorMessage { get; private set; }

        public void Error(Token token, string message)
        {
            HadError = true;
            LastErrorMessage = message;
        }

        public void Error(int line, int column, string message)
        {
            HadError = true;
            LastErrorMessage = message;
        }

        public void RuntimeError(RuntimeError error)
        {
            HadRuntimeError = true;
            LastErrorMessage = error.Message;
        }

        public void RuntimeError(MdaThrowable error)
        {
            HadRuntimeError = true;
            LastErrorMessage = error.Message;
        }

        public void Reset()
        {
            HadError = false;
            HadRuntimeError = false;
            LastErrorMessage = null;
        }
    }

    private TestErrorReporter _errorReporter;
    private MDA.Interpreter _interpreter;

    public InterpreterTests()
    {
        _errorReporter = new TestErrorReporter();
        _interpreter = new MDA.Interpreter();
        Mda.SetErrorReporter(_errorReporter);
    }

    // Literal Evaluation
    [Fact]
    public void Evaluate_NumberLiteral()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Literal(42.0);

        var result = expr.Accept(interpreter);

        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Evaluate_StringLiteral()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Literal("hello");

        var result = expr.Accept(interpreter);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void Evaluate_BooleanLiteral()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Literal(true);

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_NullLiteral()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Literal(null);

        var result = expr.Accept(interpreter);

        Assert.Null(result);
    }

    // Arithmetic Operations
    [Fact]
    public void Evaluate_Addition_Numbers()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(3.0),
            new Token(TokenType.PLUS, "+", null, 1, 1),
            new Expr.Literal(4.0)
        );

        var result = expr.Accept(interpreter);

        Assert.Equal(7.0, result);
    }

    [Fact]
    public void Evaluate_Addition_StringConcatenation()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal("hello"),
            new Token(TokenType.PLUS, "+", null, 1, 1),
            new Expr.Literal(" world")
        );

        var result = expr.Accept(interpreter);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void Evaluate_Addition_InvalidTypes_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(true),
            new Token(TokenType.PLUS, "+", null, 1, 1),
            new Expr.Literal(42.0)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Evaluate_Subtraction()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(7.0),
            new Token(TokenType.MINUS, "-", null, 1, 1),
            new Expr.Literal(3.0)
        );

        var result = expr.Accept(interpreter);

        Assert.Equal(4.0, result);
    }

    [Fact]
    public void Evaluate_Multiplication()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(3.0),
            new Token(TokenType.STAR, "*", null, 1, 1),
            new Expr.Literal(4.0)
        );

        var result = expr.Accept(interpreter);

        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Evaluate_Division()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(10.0),
            new Token(TokenType.SLASH, "/", null, 1, 1),
            new Expr.Literal(2.0)
        );

        var result = expr.Accept(interpreter);

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Evaluate_DivisionByZero_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(10.0),
            new Token(TokenType.SLASH, "/", null, 1, 1),
            new Expr.Literal(0.0)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Evaluate_UnaryMinus()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Unary(
            new Token(TokenType.MINUS, "-", null, 1, 1),
            new Expr.Literal(42.0)
        );

        var result = expr.Accept(interpreter);

        Assert.Equal(-42.0, result);
    }

    [Fact]
    public void Evaluate_ChainedArithmetic()
    {
        var interpreter = new MDA.Interpreter();
        // Evaluates: 2 * (3 + 4) - 1
        var expr = new Expr.Binary(
            new Expr.Binary(
                new Expr.Literal(2.0),
                new Token(TokenType.STAR, "*", null, 1, 1),
                new Expr.Binary(
                    new Expr.Literal(3.0),
                    new Token(TokenType.PLUS, "+", null, 1, 1),
                    new Expr.Literal(4.0)
                )
            ),
            new Token(TokenType.MINUS, "-", null, 1, 1),
            new Expr.Literal(1.0)
        );

        var result = expr.Accept(interpreter);

        Assert.Equal(13.0, result); // 2 * (3 + 4) - 1 = 2 * 7 - 1 = 14 - 1 = 13
    }

    // Comparison and Equality
    [Fact]
    public void Evaluate_GreaterThan()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(5.0),
            new Token(TokenType.GREATER, ">", null, 1, 1),
            new Expr.Literal(3.0)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_GreaterThanOrEqual()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(5.0),
            new Token(TokenType.GREATER_EQUAL, ">=", null, 1, 1),
            new Expr.Literal(5.0)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_LessThan()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(3.0),
            new Token(TokenType.LESS, "<", null, 1, 1),
            new Expr.Literal(5.0)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_LessThanOrEqual()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(5.0),
            new Token(TokenType.LESS_EQUAL, "<=", null, 1, 1),
            new Expr.Literal(5.0)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_Equality_SameType()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(5.0),
            new Token(TokenType.EQUAL_EQUAL, "==", null, 1, 1),
            new Expr.Literal(5.0)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_Equality_DifferentTypes()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(5.0),
            new Token(TokenType.EQUAL_EQUAL, "==", null, 1, 1),
            new Expr.Literal("5.0")
        );

        var result = expr.Accept(interpreter);

        Assert.False((bool)result!);
    }

    [Fact]
    public void Evaluate_Inequality()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(5.0),
            new Token(TokenType.BANG_EQUAL, "!=", null, 1, 1),
            new Expr.Literal(3.0)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_NullEquality()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(null),
            new Token(TokenType.EQUAL_EQUAL, "==", null, 1, 1),
            new Expr.Literal(null)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    // Logical Operations
    [Fact]
    public void Evaluate_LogicalAnd_BothTrue()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Logical(
            new Expr.Literal(true),
            new Token(TokenType.AND, "and", null, 1, 1),
            new Expr.Literal(true)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_LogicalAnd_FirstFalse_ShortCircuit()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Logical(
            new Expr.Literal(false),
            new Token(TokenType.AND, "and", null, 1, 1),
            new Expr.Literal(true)
        );

        var result = expr.Accept(interpreter);

        Assert.False((bool)result!);
    }

    [Fact]
    public void Evaluate_LogicalOr_BothFalse()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Logical(
            new Expr.Literal(false),
            new Token(TokenType.OR, "or", null, 1, 1),
            new Expr.Literal(false)
        );

        var result = expr.Accept(interpreter);

        Assert.False((bool)result!);
    }

    [Fact]
    public void Evaluate_LogicalOr_FirstTrue_ShortCircuit()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Logical(
            new Expr.Literal(true),
            new Token(TokenType.OR, "or", null, 1, 1),
            new Expr.Literal(false)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_LogicalNot()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Unary(
            new Token(TokenType.BANG, "!", null, 1, 1),
            new Expr.Literal(false)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    [Fact]
    public void Evaluate_ChainedLogical()
    {
        var interpreter = new MDA.Interpreter();
        // Evaluates: true and false or true
        var expr = new Expr.Logical(
            new Expr.Logical(
                new Expr.Literal(true),
                new Token(TokenType.AND, "and", null, 1, 1),
                new Expr.Literal(false)
            ),
            new Token(TokenType.OR, "or", null, 1, 1),
            new Expr.Literal(true)
        );

        var result = expr.Accept(interpreter);

        Assert.True((bool)result!);
    }

    // Variables
    [Fact]
    public void Execute_VariableDeclaration_NoInitializer()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
            null
        );

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Null(result);
    }

    [Fact]
    public void Execute_VariableDeclaration_WithInitializer()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
            new Expr.Literal(42.0)
        );

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_VariableAssignment()
    {
        var interpreter = new MDA.Interpreter();
        // First declare the variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
            new Expr.Literal(42.0)
        );
        declStmt.Accept(interpreter);

        // Then assign a new value
        var assignExpr = new Expr.Assign(
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
            new Expr.Literal(24.0)
        );
        var result = assignExpr.Accept(interpreter);

        Assert.Equal(24.0, result);

        // Verify the new value is stored
        var readExpr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1));
        result = readExpr.Accept(interpreter);
        Assert.Equal(24.0, result);
    }

    [Fact]
    public void Execute_VariableAccess_Undefined_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1));

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_GlobalVariable()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "global", "global", 1, 1),
            new Expr.Literal(42.0)
        );

        stmt.Accept(interpreter);

        // Create a new environment for local scope
        var env = new Environment();
        interpreter.ExecuteBlock(new List<Stmt>(), env);

        // Variable should still be accessible
        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "global", "global", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_LocalVariable()
    {
        var interpreter = new MDA.Interpreter();
        var localVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "local", "local", 1, 1),
            new Expr.Literal(42.0)
        );

        // Create a new environment for local scope
        var env = new Environment();
        interpreter.ExecuteBlock(new List<Stmt> { localVar }, env);

        // Variable should not be accessible in global scope
        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "local", "local", 1, 1));
        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    // Control Flow
    [Fact]
    public void Execute_If_TrueCondition()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.If(
            new Expr.Literal(true),
            new Stmt.Expression(new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            )),
            null
        );

        // First declare the variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(0.0)
        );
        declStmt.Accept(interpreter);

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_If_FalseCondition()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.If(
            new Expr.Literal(false),
            new Stmt.Expression(new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            )),
            null
        );

        // First declare the variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(0.0)
        );
        declStmt.Accept(interpreter);

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(0.0, result); // Value should remain unchanged
    }

    [Fact]
    public void Execute_If_NonBooleanCondition()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.If(
            new Expr.Literal(42.0), // Non-boolean condition
            new Stmt.Expression(new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            )),
            null
        );

        // First declare the variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(0.0)
        );
        declStmt.Accept(interpreter);

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(42.0, result); // Non-zero numbers are truthy
    }

    [Fact]
    public void Execute_IfElse()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.If(
            new Expr.Literal(false),
            new Stmt.Expression(new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            )),
            new Stmt.Expression(new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(24.0)
            ))
        );

        // First declare the variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(0.0)
        );
        declStmt.Accept(interpreter);

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(24.0, result); // Else branch should execute
    }

    [Fact]
    public void Execute_While_ZeroIterations()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.While(
            new Expr.Literal(false),
            new Stmt.Expression(new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            )),
            null // No increment
        );

        // First declare the variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(0.0)
        );
        declStmt.Accept(interpreter);

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(0.0, result); // Value should remain unchanged
    }

    [Fact]
    public void Execute_While_MultipleIterations()
    {
        var interpreter = new MDA.Interpreter();

        // First declare the counter variable
        var declCounter = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1),
            new Expr.Literal(0.0)
        );
        declCounter.Accept(interpreter);

        // Create while loop that increments counter until it reaches 3
        var stmt = new Stmt.While(
            new Expr.Binary(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1)),
                new Token(TokenType.LESS, "<", null, 1, 1),
                new Expr.Literal(3.0)
            ),
            new Stmt.Expression(new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1),
                new Expr.Binary(
                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1)),
                    new Token(TokenType.PLUS, "+", null, 1, 1),
                    new Expr.Literal(1.0)
                )
            )),
            null // No increment since it's in the body
        );

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Execute_While_Break()
    {
        var interpreter = new MDA.Interpreter();

        // First declare the counter variable
        var declCounter = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1),
            new Expr.Literal(0.0)
        );
        declCounter.Accept(interpreter);

        // Create while loop that increments counter but breaks at 2
        var stmt = new Stmt.While(
            new Expr.Literal(true), // Infinite loop
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Expression(new Expr.Assign(
                    new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1),
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1)),
                        new Token(TokenType.PLUS, "+", null, 1, 1),
                        new Expr.Literal(1.0)
                    )
                )),
                new Stmt.If(
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1)),
                        new Token(TokenType.GREATER_EQUAL, ">=", null, 1, 1),
                        new Expr.Literal(2.0)
                    ),
                    new Stmt.Break(new Token(TokenType.BREAK, "break", null, 1, 1)),
                    null
                )
            }),
            null // No increment since it's in the body
        );

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void Execute_For_AllComponents()
    {
        var interpreter = new MDA.Interpreter();

        var iToken = new Token(TokenType.IDENTIFIER, "i", "i", 1, 1);
        var declStmt = new Stmt.Var(iToken, new Expr.Literal(0.0));

        // Create a for loop that counts from 0 to 2
        var stmt = new Stmt.Block(new List<Stmt>
        {
            // Initializer
            new Stmt.Expression(
                new Expr.Assign(iToken, new Expr.Literal(0.0))
            ),
            // While loop
            new Stmt.While(
                // Condition
                new Expr.Binary(
                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                    new Token(TokenType.LESS, "<", null, 1, 1),
                    new Expr.Literal(3.0)
                ),
                // Body
                new Stmt.Block(new List<Stmt>
                {
                    new Stmt.Expression(new Expr.Assign(
                        new Token(TokenType.IDENTIFIER, "i", "i", 1, 1),
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                            new Token(TokenType.PLUS, "+", null, 1, 1),
                            new Expr.Literal(1.0)
                        )
                    ))
                }),
                // Increment
                new Expr.Assign(
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 1),
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                        new Token(TokenType.PLUS, "+", null, 1, 1),
                        new Expr.Literal(1.0)
                    )
                )
            )
        });

        declStmt.Accept(interpreter);
        stmt.Accept(interpreter);

        var expr = new Expr.Variable(iToken);
        var result = expr.Accept(interpreter);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Execute_For_NoComponents()
    {
        var interpreter = new MDA.Interpreter();

        // Create a for loop with no initializer, condition, or increment
        var stmt = new Stmt.Block(new List<Stmt>
        {
            new Stmt.While(
                new Expr.Literal(true),
                new Stmt.Block(new List<Stmt>
                {
                    new Stmt.Break(new Token(TokenType.BREAK, "break", null, 1, 1))
                }),
                null // No increment
            )
        });

        stmt.Accept(interpreter);
        // Test passes if no exception is thrown
    }

    [Fact]
    public void Execute_For_Break()
    {
        var interpreter = new MDA.Interpreter();

        // First declare the counter and i variables
        var declCounter = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1),
            new Expr.Literal(0.0)
        );

        var iToken = new Token(TokenType.IDENTIFIER, "i", "i", 1, 1);
        var declI = new Stmt.Var(iToken, new Expr.Literal(0.0));
        declCounter.Accept(interpreter);
        declI.Accept(interpreter);

        // Create a for loop that breaks at counter = 2
        var stmt = new Stmt.Block(new List<Stmt>
        {
            new Stmt.Expression(
                new Expr.Assign(iToken, new Expr.Literal(0.0))
            ),
            new Stmt.While(
                new Expr.Binary(
                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                    new Token(TokenType.LESS, "<", null, 1, 1),
                    new Expr.Literal(5.0)
                ),
                new Stmt.Block(new List<Stmt>
                {
                    new Stmt.Expression(new Expr.Assign(
                        new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1),
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1)),
                            new Token(TokenType.PLUS, "+", null, 1, 1),
                            new Expr.Literal(1.0)
                        )
                    )),
                    new Stmt.If(
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1)),
                            new Token(TokenType.GREATER_EQUAL, ">=", null, 1, 1),
                            new Expr.Literal(2.0)
                        ),
                        new Stmt.Break(new Token(TokenType.BREAK, "break", null, 1, 1)),
                        null
                    ),
                    new Stmt.Expression(new Expr.Assign(
                        new Token(TokenType.IDENTIFIER, "i", "i", 1, 1),
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                            new Token(TokenType.PLUS, "+", null, 1, 1),
                            new Expr.Literal(1.0)
                        )
                    ))
                }),
                null // No increment since it's in the body
            )
        });

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter", "counter", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void Execute_For_Continue()
    {
        var interpreter = new MDA.Interpreter();

        // First declare the sum variable
        var declSum = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "sum", "sum", 1, 1),
            new Expr.Literal(0.0)
        );
        var iToken = new Token(TokenType.IDENTIFIER, "i", "i", 1, 1);
        var declI = new Stmt.Var(
            iToken,
            new Expr.Literal(0.0)
        );
        declSum.Accept(interpreter);
        declI.Accept(interpreter);

        // Create a for loop that skips odd numbers
        var stmt = new Stmt.Block(new List<Stmt>
        {
            // new Stmt.Expression(new Expr.Assign(iToken, new Expr.Literal(0.0))),
            new Stmt.While(
                new Expr.Binary(
                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                    new Token(TokenType.LESS, "<", null, 1, 1),
                    new Expr.Literal(5.0)
                ),
                new Stmt.Block(new List<Stmt>
                {
                    // Skip odd numbers
                    new Stmt.If(
                        new Expr.Binary(
                            new Expr.Binary(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                                new Token(TokenType.PERCENT, "%", null, 1, 1),
                                new Expr.Literal(2.0)
                            ),
                            new Token(TokenType.EQUAL_EQUAL, "==", null, 1, 1),
                            new Expr.Literal(1.0)
                        ),
                        new Stmt.Continue(new Token(TokenType.CONTINUE, "continue", null, 1, 1)),
                        null
                    ),
                    // Add even numbers to sum
                    new Stmt.Expression(new Expr.Assign(
                        new Token(TokenType.IDENTIFIER, "sum", "sum", 1, 1),
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "sum", "sum", 1, 1)),
                            new Token(TokenType.PLUS, "+", null, 1, 1),
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1))
                        )
                    )),
                    // Increment i
                    new Stmt.Expression(new Expr.Assign(
                        new Token(TokenType.IDENTIFIER, "i", "i", 1, 1),
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                            new Token(TokenType.PLUS, "+", null, 1, 1),
                            new Expr.Literal(1.0)
                        )
                    ))
                }),
                new Expr.Assign(
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 1),
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "i", "i", 1, 1)),
                        new Token(TokenType.PLUS, "+", null, 1, 1),
                        new Expr.Literal(1.0)
                    )
                )
            )
        });

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "sum", "sum", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(6.0, result); // 0 + 2 + 4 = 6
    }

    // Functions
    [Fact]
    public void Execute_FunctionDeclaration()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            new List<Token>(),
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 1, 1),
                    new Expr.Literal(42.0)
                )
            }
        );

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.IsType<MdaFunction>(result);
    }

    [Fact]
    public void Execute_FunctionCall_NoParameters()
    {
        var interpreter = new MDA.Interpreter();
        // First declare the function
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            new List<Token>(),
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 1, 1),
                    new Expr.Literal(42.0)
                )
            }
        );
        declStmt.Accept(interpreter);

        // Then call it
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_FunctionCall_WithParameters()
    {
        var interpreter = new MDA.Interpreter();
        // First declare the function
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "add", "add", 1, 1),
            new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
                new Token(TokenType.IDENTIFIER, "b", "b", 1, 1)
            },
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 1, 1),
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1)),
                        new Token(TokenType.PLUS, "+", null, 1, 1),
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "b", "b", 1, 1))
                    )
                )
            }
        );
        declStmt.Accept(interpreter);

        // Then call it
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "add", "add", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>
            {
                new Expr.Literal(3.0),
                new Expr.Literal(4.0)
            }
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void Execute_FunctionCall_WrongArgumentCount_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        // First declare the function
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "add", "add", 1, 1),
            new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
                new Token(TokenType.IDENTIFIER, "b", "b", 1, 1)
            },
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 1, 1),
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1)),
                        new Token(TokenType.PLUS, "+", null, 1, 1),
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "b", "b", 1, 1))
                    )
                )
            }
        );
        declStmt.Accept(interpreter);

        // Then call it with wrong number of arguments
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "add", "add", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>
            {
                new Expr.Literal(3.0)
            }
        );

        Assert.Throws<RuntimeError>(() => callExpr.Accept(interpreter));
    }

    [Fact]
    public void Execute_FunctionReturn_ExplicitValue()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            new List<Token>(),
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 1, 1),
                    new Expr.Literal(42.0)
                )
            }
        );
        stmt.Accept(interpreter);

        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_FunctionReturn_ImplicitNull()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            new List<Token>(),
            new List<Stmt>
            {
                // No return statement
            }
        );
        stmt.Accept(interpreter);

        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.Null(result);
    }

    [Fact]
    public void Execute_Recursion()
    {
        var interpreter = new MDA.Interpreter();
        // Declare factorial function
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "factorial", "factorial", 1, 1),
            new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)
            },
            new List<Stmt>
            {
                new Stmt.If(
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)),
                        new Token(TokenType.LESS_EQUAL, "<=", null, 1, 1),
                        new Expr.Literal(1.0)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 1, 1),
                        new Expr.Literal(1.0)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 1, 1),
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)),
                            new Token(TokenType.STAR, "*", null, 1, 1),
                            new Expr.Call(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "factorial", "factorial", 1, 1)),
                                new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                                new List<Expr>
                                {
                                    new Expr.Binary(
                                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)),
                                        new Token(TokenType.MINUS, "-", null, 1, 1),
                                        new Expr.Literal(1.0)
                                    )
                                }
                            )
                        )
                    )
                )
            }
        );
        declStmt.Accept(interpreter);

        // Call factorial(5)
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "factorial", "factorial", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>
            {
                new Expr.Literal(5.0)
            }
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(120.0, result); // 5! = 5 * 4 * 3 * 2 * 1 = 120
    }

    [Fact]
    public void Execute_NestedFunctions()
    {
        var interpreter = new MDA.Interpreter();
        // Declare outer function that returns inner function
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "makeAdder", "makeAdder", 1, 1),
            new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1)
            },
            new List<Stmt>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "adder", "adder", 2, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "y", "y", 2, 1)
                    },
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 2, 1),
                            new Expr.Binary(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 2, 1)),
                                new Token(TokenType.PLUS, "+", null, 2, 1),
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "y", "y", 2, 1))
                            )
                        )
                    }
                ),
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 1, 1),
                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "adder", "adder", 1, 1))
                )
            }
        );
        declStmt.Accept(interpreter);

        // Create an adder that adds 5
        var makeAdderCall = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "makeAdder", "makeAdder", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>
            {
                new Expr.Literal(5.0)
            }
        );

        var add5 = makeAdderCall.Accept(interpreter);
        Assert.IsType<MdaFunction>(add5);

        // Use the adder
        var addCall = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "add5", "add5", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>
            {
                new Expr.Literal(3.0)
            }
        );

        // Store the adder in a variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "add5", "add5", 1, 1),
            makeAdderCall
        );
        declVar.Accept(interpreter);

        var result = addCall.Accept(interpreter);
        Assert.Equal(8.0, result); // 5 + 3 = 8
    }

    [Fact]
    public void Execute_Closure_CapturesEnvironment()
    {
        var interpreter = new MDA.Interpreter();
        // Declare a variable in the outer scope
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(42.0)
        );
        declVar.Accept(interpreter);

        // Declare a function that captures x
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "getX", "getX", 1, 1),
            new List<Token>(),
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 1, 1),
                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 1))
                )
            }
        );
        declStmt.Accept(interpreter);

        // Call getX()
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "getX", "getX", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_Closure_ModifiesEnclosingVariable()
    {
        var interpreter = new MDA.Interpreter();
        // Declare a variable in the outer scope
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(42.0)
        );
        declVar.Accept(interpreter);

        // Declare a function that modifies x
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "setX", "setX", 1, 1),
            new List<Token>(),
            new List<Stmt>
            {
                new Stmt.Expression(
                    new Expr.Assign(
                        new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                        new Expr.Literal(24.0)
                    )
                )
            }
        );
        declStmt.Accept(interpreter);

        // Call setX()
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "setX", "setX", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );
        callExpr.Accept(interpreter);

        // Check that x was modified
        var readExpr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 1));
        var result = readExpr.Accept(interpreter);
        Assert.Equal(24.0, result);
    }

    // Classes
    [Fact]
    public void Execute_ClassDeclaration()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null, // No superclass
            new List<Stmt.Function>() // No methods
        );

        stmt.Accept(interpreter);

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1));
        var result = expr.Accept(interpreter);
        Assert.IsType<MdaClass>(result);
    }

    [Fact]
    public void Execute_InstanceCreation()
    {
        var interpreter = new MDA.Interpreter();
        // First declare the class
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>()
        );
        classStmt.Accept(interpreter);

        // Then create an instance
        var expr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = expr.Accept(interpreter);
        Assert.IsType<MdaInstance>(result);
    }

    [Fact]
    public void Execute_MethodCall()
    {
        var interpreter = new MDA.Interpreter();
        // Declare class with a method
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 2, 1),
                            new Expr.Literal(42.0)
                        )
                    }
                )
            }
        );
        classStmt.Accept(interpreter);

        // Create instance
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Call method
        var methodCall = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
        );
        var callExpr = new Expr.Call(
            methodCall,
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_PropertyGet()
    {
        var interpreter = new MDA.Interpreter();
        // Declare class
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>()
        );
        classStmt.Accept(interpreter);

        // Create instance
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Set property
        var setExpr = new Expr.Set(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.IDENTIFIER, "prop", "prop", 1, 1),
            new Expr.Literal(42.0)
        );
        setExpr.Accept(interpreter);

        // Get property
        var getExpr = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.IDENTIFIER, "prop", "prop", 1, 1)
        );

        var result = getExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_PropertySet()
    {
        var interpreter = new MDA.Interpreter();
        // Declare class
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>()
        );
        classStmt.Accept(interpreter);

        // Create instance
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Set property
        var setExpr = new Expr.Set(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.IDENTIFIER, "prop", "prop", 1, 1),
            new Expr.Literal(42.0)
        );
        var result = setExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_This_InMethod()
    {
        var interpreter = new MDA.Interpreter();
        // Declare class with a method that uses 'this'
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 2, 1),
                            new Expr.This(new Token(TokenType.THIS, "this", null, 2, 1))
                        )
                    }
                )
            }
        );
        classStmt.Accept(interpreter);

        // Create instance
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Call method
        var methodCall = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
        );
        var callExpr = new Expr.Call(
            methodCall,
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.IsType<MdaInstance>(result);
    }

    [Fact]
    public void Execute_This_OutsideMethod_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.This(new Token(TokenType.THIS, "this", null, 1, 1));

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_Inheritance_MethodInheritance()
    {
        var interpreter = new MDA.Interpreter();
        // Declare superclass with a method
        var superclassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Super", "Super", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 2, 1),
                            new Expr.Literal(42.0)
                        )
                    }
                )
            }
        );
        superclassStmt.Accept(interpreter);

        // Declare subclass
        var subclassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Sub", "Sub", 3, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Super", "Super", 3, 1)),
            new List<Stmt.Function>()
        );
        subclassStmt.Accept(interpreter);

        // Create instance of subclass
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Sub", "Sub", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "sub", "sub", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Call inherited method
        var methodCall = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "sub", "sub", 1, 1)),
            new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
        );
        var callExpr = new Expr.Call(
            methodCall,
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_Super_CallsSuperMethod()
    {
        var interpreter = new MDA.Interpreter();
        // Declare superclass with a method
        var superclassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Super", "Super", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 2, 1),
                            new Expr.Literal(42.0)
                        )
                    }
                )
            }
        );
        superclassStmt.Accept(interpreter);

        // Declare subclass that overrides the method but calls super
        var subclassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Sub", "Sub", 3, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Super", "Super", 3, 1)),
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 4, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 4, 1),
                            new Expr.Call(
                                new Expr.Super(
                                    new Token(TokenType.SUPER, "super", null, 4, 1),
                                    new Token(TokenType.IDENTIFIER, "method", "method", 4, 1)
                                ),
                                new Token(TokenType.LEFT_PAREN, "(", null, 4, 1),
                                new List<Expr>()
                            )
                        )
                    }
                )
            }
        );
        subclassStmt.Accept(interpreter);

        // Create instance of subclass
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Sub", "Sub", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "sub", "sub", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Call method that uses super
        var methodCall = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "sub", "sub", 1, 1)),
            new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
        );
        var callExpr = new Expr.Call(
            methodCall,
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Execute_Super_OutsideSubclass_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Super(
            new Token(TokenType.SUPER, "super", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_Constructor_WithInitializer()
    {
        var interpreter = new MDA.Interpreter();
        // Declare class with init method
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 2, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "value", "value", 2, 1)
                    },
                    new List<Stmt>
                    {
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 2, 1)),
                            new Token(TokenType.IDENTIFIER, "value", "value", 2, 1),
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "value", "value", 2, 1))
                        ))
                    }
                )
            }
        );
        classStmt.Accept(interpreter);

        // Create instance with constructor argument
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>
            {
                new Expr.Literal(42.0)
            }
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Get initialized property
        var getExpr = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.IDENTIFIER, "value", "value", 1, 1)
        );

        var result = getExpr.Accept(interpreter);
        Assert.Equal(42.0, result);
    }

    // Resolution and Scoping
    [Fact]
    public void Resolve_LocalVariable()
    {
        var interpreter = new MDA.Interpreter();
        // Create a block with a local variable
        var stmt = new Stmt.Block(new List<Stmt>
        {
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            ),
            new Stmt.Expression(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 1, 2))
            )
        });

        // Resolve the variable reference
        var varExpr = ((Stmt.Expression)stmt.Statements[1]).Expr;
        interpreter.Resolve(varExpr, 0);

        stmt.Accept(interpreter);
        // Test passes if no exception is thrown
    }

    [Fact]
    public void Resolve_GlobalVariable()
    {
        var interpreter = new MDA.Interpreter();
        // Declare global variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "global", "global", 1, 1),
            new Expr.Literal(42.0)
        );
        declStmt.Accept(interpreter);

        // Reference it from a block
        var varExpr = new Stmt.Block(new List<Stmt>
        {
            new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "global", "global", 1, 1)))
        });

        varExpr.Accept(interpreter);
        var exception = Record.Exception(() => varExpr.Accept(interpreter)); // Test passes if no exception is thrown()
        Assert.Null(exception);
    }

    [Fact]
    public void Resolve_UninitializedVariable_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "undefined", "undefined", 1, 1));

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Resolve_InvalidThis_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.This(new Token(TokenType.THIS, "this", null, 1, 1));

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Resolve_InvalidSuper_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Super(
            new Token(TokenType.SUPER, "super", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    // Runtime Errors
    [Fact]
    public void RuntimeError_UndefinedVariable()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "undefined", "undefined", 1, 1));

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void RuntimeError_InvalidOperandType()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal("not a number"),
            new Token(TokenType.MINUS, "-", null, 1, 1),
            new Expr.Literal(42.0)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void RuntimeError_UndefinedProperty()
    {
        var interpreter = new MDA.Interpreter();
        // Declare a class
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>()
        );
        classStmt.Accept(interpreter);

        // Create instance
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Try to access undefined property
        var getExpr = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.IDENTIFIER, "undefined", "undefined", 1, 1)
        );

        Assert.Throws<RuntimeError>(() => getExpr.Accept(interpreter));
    }

    [Fact]
    public void RuntimeError_NotCallable()
    {
        var interpreter = new MDA.Interpreter();
        // Try to call a number
        var callExpr = new Expr.Call(
            new Expr.Literal(42.0),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        Assert.Throws<RuntimeError>(() => callExpr.Accept(interpreter));
    }

    // Native Functions
    [Fact]
    public void Execute_Clock_ReturnsCurrentTime()
    {
        var interpreter = new MDA.Interpreter();
        var clockExpr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "clock", "clock", 1, 1));
        var clock = clockExpr.Accept(interpreter);
        Assert.IsType<NativeFunction>(clock);

        var callExpr = new Expr.Call(
            clockExpr,
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );

        var result = callExpr.Accept(interpreter);
        Assert.IsType<double>(result);
    }

    [Fact]
    public void Execute_Print_OutputsValue()
    {
        var interpreter = new MDA.Interpreter();
        var printExpr = new Stmt.Print(new Expr.Literal(1.0));
        var print = printExpr.Accept(interpreter);

        var result = printExpr.Accept(interpreter);
        Assert.Null(result);
    }

    // State Management
    [Fact]
    public void Interpreter_MaintainsGlobalState()
    {
        var interpreter = new MDA.Interpreter();
        // Define a global variable
        var declStmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "global", "global", 1, 1),
            new Expr.Literal(42.0)
        );
        declStmt.Accept(interpreter);

        // Create a new environment and modify the global
        var env = new Environment();
        var block = new List<Stmt>
        {
            new Stmt.Expression(
                new Expr.Assign(
                    new Token(TokenType.IDENTIFIER, "global", "global", 2, 1),
                    new Expr.Literal(24.0)
                )
            )
        };
        interpreter.ExecuteBlock(block, env);

        // Check that the global was modified
        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "global", "global", 3, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(24.0, result);
    }

    [Fact]
    public void Interpreter_HandlesRepeatedExecution()
    {
        var interpreter = new MDA.Interpreter();
        var stmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(42.0)
        );

        // Execute the same statement multiple times
        stmt.Accept(interpreter);
        Assert.Throws<RuntimeError>(() => stmt.Accept(interpreter)); // Can't redeclare variable

        // But can reassign
        var assignStmt = new Stmt.Expression(
            new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "x", "x", 2, 1),
                new Expr.Literal(24.0)
            )
        );
        assignStmt.Accept(interpreter);
        assignStmt.Accept(interpreter); // Can reassign multiple times

        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 3, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(24.0, result);
    }

    // Complex Programs
    [Fact]
    public void Execute_ComplexProgram_Fibonacci()
    {
        var interpreter = new MDA.Interpreter();
        // Declare fibonacci function
        var fibStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "fib", "fib", 1, 1),
            new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)
            },
            new List<Stmt>
            {
                new Stmt.If(
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 2, 1)),
                        new Token(TokenType.LESS_EQUAL, "<=", null, 2, 1),
                        new Expr.Literal(1.0)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 3, 1),
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 3, 1))
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 5, 1),
                        new Expr.Binary(
                            new Expr.Call(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "fib", "fib", 5, 1)),
                                new Token(TokenType.LEFT_PAREN, "(", null, 5, 1),
                                new List<Expr>
                                {
                                    new Expr.Binary(
                                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 5, 1)),
                                        new Token(TokenType.MINUS, "-", null, 5, 1),
                                        new Expr.Literal(1.0)
                                    )
                                }
                            ),
                            new Token(TokenType.PLUS, "+", null, 5, 1),
                            new Expr.Call(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "fib", "fib", 5, 1)),
                                new Token(TokenType.LEFT_PAREN, "(", null, 5, 1),
                                new List<Expr>
                                {
                                    new Expr.Binary(
                                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 5, 1)),
                                        new Token(TokenType.MINUS, "-", null, 5, 1),
                                        new Expr.Literal(2.0)
                                    )
                                }
                            )
                        )
                    )
                )
            }
        );
        fibStmt.Accept(interpreter);

        // Call fib(5)
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "fib", "fib", 6, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 6, 1),
            new List<Expr> { new Expr.Literal(5.0) }
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(5.0, result); // fib(5) = fib(4) + fib(3) = 3 + 2 = 5
    }

    [Fact]
    public void Execute_ComplexProgram_ClassHierarchy()
    {
        var interpreter = new MDA.Interpreter();
        // Create a base shape class
        var shapeClassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Shape", "Shape", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 2, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 2, 1)),
                            new Token(TokenType.IDENTIFIER, "area", "area", 2, 1),
                            new Expr.Literal(0.0)
                        ))
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "getArea", "getArea", 3, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 3, 1),
                            new Expr.Get(
                                new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                                new Token(TokenType.IDENTIFIER, "area", "area", 3, 1)
                            )
                        )
                    }
                )
            }
        );
        shapeClassStmt.Accept(interpreter);

        // Create a circle class that inherits from shape
        var circleClassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Circle", "Circle", 4, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Shape", "Shape", 4, 1)),
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 5, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "radius", "radius", 5, 1)
                    },
                    new List<Stmt>
                    {
                        // Call super.init()
                        new Stmt.Expression(new Expr.Call(
                            new Expr.Super(
                                new Token(TokenType.SUPER, "super", null, 5, 1),
                                new Token(TokenType.IDENTIFIER, "init", "init", 5, 1)
                            ),
                            new Token(TokenType.LEFT_PAREN, "(", null, 5, 1),
                            new List<Expr>()
                        )),
                        // Set radius
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 5, 1)),
                            new Token(TokenType.IDENTIFIER, "radius", "radius", 5, 1),
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "radius", "radius", 5, 1))
                        )),
                        // Calculate and set area
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 5, 1)),
                            new Token(TokenType.IDENTIFIER, "area", "area", 5, 1),
                            new Expr.Binary(
                                new Expr.Binary(
                                    new Expr.Literal(3.14159),
                                    new Token(TokenType.STAR, "*", null, 5, 1),
                                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "radius", "radius", 5, 1))
                                ),
                                new Token(TokenType.STAR, "*", null, 5, 1),
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "radius", "radius", 5, 1))
                            )
                        ))
                    }
                )
            }
        );
        circleClassStmt.Accept(interpreter);

        // Create a rectangle class that inherits from shape
        var rectangleClassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Rectangle", "Rectangle", 6, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Shape", "Shape", 6, 1)),
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 7, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "width", "width", 7, 1),
                        new Token(TokenType.IDENTIFIER, "height", "height", 7, 1)
                    },
                    new List<Stmt>
                    {
                        // Call super.init()
                        new Stmt.Expression(new Expr.Call(
                            new Expr.Super(
                                new Token(TokenType.SUPER, "super", null, 7, 1),
                                new Token(TokenType.IDENTIFIER, "init", "init", 7, 1)
                            ),
                            new Token(TokenType.LEFT_PAREN, "(", null, 7, 1),
                            new List<Expr>()
                        )),
                        // Set width and height
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 7, 1)),
                            new Token(TokenType.IDENTIFIER, "width", "width", 7, 1),
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "width", "width", 7, 1))
                        )),
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 7, 1)),
                            new Token(TokenType.IDENTIFIER, "height", "height", 7, 1),
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "height", "height", 7, 1))
                        )),
                        // Calculate and set area
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 7, 1)),
                            new Token(TokenType.IDENTIFIER, "area", "area", 7, 1),
                            new Expr.Binary(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "width", "width", 7, 1)),
                                new Token(TokenType.STAR, "*", null, 7, 1),
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "height", "height", 7, 1))
                            )
                        ))
                    }
                )
            }
        );
        rectangleClassStmt.Accept(interpreter);

        // Create instances
        var circle = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Circle", "Circle", 8, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 8, 1),
            new List<Expr> { new Expr.Literal(2.0) }
        );

        var rectangle = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Rectangle", "Rectangle", 9, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 9, 1),
            new List<Expr> { new Expr.Literal(3.0), new Expr.Literal(4.0) }
        );

        // Store instances in variables
        var declCircle = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "circle", "circle", 10, 1),
            circle
        );
        declCircle.Accept(interpreter);

        var declRectangle = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "rectangle", "rectangle", 11, 1),
            rectangle
        );
        declRectangle.Accept(interpreter);

        // Get areas
        var circleArea = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "circle", "circle", 12, 1)),
                new Token(TokenType.IDENTIFIER, "getArea", "getArea", 12, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 12, 1),
            new List<Expr>()
        );

        var rectangleArea = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "rectangle", "rectangle", 13, 1)),
                new Token(TokenType.IDENTIFIER, "getArea", "getArea", 13, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 13, 1),
            new List<Expr>()
        );

        var circleResult = circleArea.Accept(interpreter);
        var rectangleResult = rectangleArea.Accept(interpreter);

        Assert.Equal(12.56636, (double)circleResult, 5); // π * 2^2
        Assert.Equal(12.0, (double)rectangleResult); // 3 * 4
    }

    [Fact]
    public void Execute_ComplexProgram_ClosureInteractions()
    {
        var interpreter = new MDA.Interpreter();
        // Create a counter factory class
        var counterFactoryStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "CounterFactory", "CounterFactory", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "createCounter", "createCounter", 2, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "start", "start", 2, 1)
                    },
                    new List<Stmt>
                    {
                        // Create a counter class with the starting value
                        new Stmt.Class(
                            new Token(TokenType.IDENTIFIER, "Counter", "Counter", 3, 1),
                            null,
                            new List<Stmt.Function>
                            {
                                new Stmt.Function(
                                    new Token(TokenType.IDENTIFIER, "init", "init", 4, 1),
                                    new List<Token>(),
                                    new List<Stmt>
                                    {
                                        new Stmt.Expression(new Expr.Set(
                                            new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                                            new Token(TokenType.IDENTIFIER, "count", "count", 4, 1),
                                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "start", "start", 4, 1))
                                        ))
                                    }
                                ),
                                new Stmt.Function(
                                    new Token(TokenType.IDENTIFIER, "increment", "increment", 5, 1),
                                    new List<Token>(),
                                    new List<Stmt>
                                    {
                                        new Stmt.Expression(new Expr.Set(
                                            new Expr.This(new Token(TokenType.THIS, "this", null, 5, 1)),
                                            new Token(TokenType.IDENTIFIER, "count", "count", 5, 1),
                                            new Expr.Binary(
                                                new Expr.Get(
                                                    new Expr.This(new Token(TokenType.THIS, "this", null, 5, 1)),
                                                    new Token(TokenType.IDENTIFIER, "count", "count", 5, 1)
                                                ),
                                                new Token(TokenType.PLUS, "+", null, 5, 1),
                                                new Expr.Literal(1.0)
                                            )
                                        ))
                                    }
                                ),
                                new Stmt.Function(
                                    new Token(TokenType.IDENTIFIER, "getCount", "getCount", 6, 1),
                                    new List<Token>(),
                                    new List<Stmt>
                                    {
                                        new Stmt.Return(
                                            new Token(TokenType.RETURN, "return", null, 6, 1),
                                            new Expr.Get(
                                                new Expr.This(new Token(TokenType.THIS, "this", null, 6, 1)),
                                                new Token(TokenType.IDENTIFIER, "count", "count", 6, 1)
                                            )
                                        )
                                    }
                                )
                            }
                        ),
                        // Return a new counter instance
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 7, 1),
                            new Expr.Call(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Counter", "Counter", 7, 1)),
                                new Token(TokenType.LEFT_PAREN, "(", null, 7, 1),
                                new List<Expr>()
                            )
                        )
                    }
                )
            }
        );
        counterFactoryStmt.Accept(interpreter);

        // Create factory instance
        var factory = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "CounterFactory", "CounterFactory", 8, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 8, 1),
            new List<Expr>()
        );

        // Store factory in variable
        var declFactory = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "factory", "factory", 9, 1),
            factory
        );
        declFactory.Accept(interpreter);

        // Create two counters with different starting values
        var counter1 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "factory", "factory", 10, 1)),
                new Token(TokenType.IDENTIFIER, "createCounter", "createCounter", 10, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 10, 1),
            new List<Expr> { new Expr.Literal(0.0) }
        );

        var counter2 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "factory", "factory", 11, 1)),
                new Token(TokenType.IDENTIFIER, "createCounter", "createCounter", 11, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 11, 1),
            new List<Expr> { new Expr.Literal(10.0) }
        );

        // Store counters in variables
        var declCounter1 = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "counter1", "counter1", 12, 1),
            counter1
        );
        declCounter1.Accept(interpreter);

        var declCounter2 = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "counter2", "counter2", 13, 1),
            counter2
        );
        declCounter2.Accept(interpreter);

        // Increment counters
        var increment1 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter1", "counter1", 14, 1)),
                new Token(TokenType.IDENTIFIER, "increment", "increment", 14, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 14, 1),
            new List<Expr>()
        );

        var increment2 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter2", "counter2", 15, 1)),
                new Token(TokenType.IDENTIFIER, "increment", "increment", 15, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 15, 1),
            new List<Expr>()
        );

        increment1.Accept(interpreter);
        increment1.Accept(interpreter);
        increment2.Accept(interpreter);

        // Get counts
        var getCount1 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter1", "counter1", 16, 1)),
                new Token(TokenType.IDENTIFIER, "getCount", "getCount", 16, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 16, 1),
            new List<Expr>()
        );

        var getCount2 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "counter2", "counter2", 17, 1)),
                new Token(TokenType.IDENTIFIER, "getCount", "getCount", 17, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 17, 1),
            new List<Expr>()
        );

        var count1 = getCount1.Accept(interpreter);
        var count2 = getCount2.Accept(interpreter);

        Assert.Equal(2.0, (double)count1); // 0 + 1 + 1
        Assert.Equal(11.0, (double)count2); // 10 + 1
    }

    [Fact]
    public void Execute_ComplexProgram_RecursiveDataStructures()
    {
        var interpreter = new MDA.Interpreter();
        // Create a linked list node class with limited depth
        var nodeClassStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Node", "Node", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 2, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "value", "value", 2, 1)
                    },
                    new List<Stmt>
                    {
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 2, 1)),
                            new Token(TokenType.IDENTIFIER, "value", "value", 2, 1),
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "value", "value", 2, 1))
                        )),
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 2, 1)),
                            new Token(TokenType.IDENTIFIER, "next", "next", 2, 1),
                            new Expr.Literal(null)
                        ))
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "append", "append", 3, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "value", "value", 3, 1),
                        new Token(TokenType.IDENTIFIER, "depth", "depth", 3, 1)
                    },
                    new List<Stmt>
                    {
                        new Stmt.If(
                            new Expr.Binary(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "depth", "depth", 3, 1)),
                                new Token(TokenType.LESS_EQUAL, "<=", null, 3, 1),
                                new Expr.Literal(0.0)
                            ),
                            new Stmt.Return(
                                new Token(TokenType.RETURN, "return", null, 3, 1),
                                new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1))
                            ),
                            new Stmt.Block(new List<Stmt>
                            {
                                new Stmt.If(
                                    new Expr.Binary(
                                        new Expr.Get(
                                            new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                                            new Token(TokenType.IDENTIFIER, "next", "next", 3, 1)
                                        ),
                                        new Token(TokenType.EQUAL_EQUAL, "==", null, 3, 1),
                                        new Expr.Literal(null)
                                    ),
                                    new Stmt.Expression(new Expr.Set(
                                        new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                                        new Token(TokenType.IDENTIFIER, "next", "next", 3, 1),
                                        new Expr.Call(
                                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Node", "Node", 3, 1)),
                                            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
                                            new List<Expr>
                                            {
                                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "value", "value", 3,
                                                    1))
                                            }
                                        )
                                    )),
                                    new Stmt.Expression(new Expr.Call(
                                        new Expr.Get(
                                            new Expr.Get(
                                                new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                                                new Token(TokenType.IDENTIFIER, "next", "next", 3, 1)
                                            ),
                                            new Token(TokenType.IDENTIFIER, "append", "append", 3, 1)
                                        ),
                                        new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
                                        new List<Expr>
                                        {
                                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "value", "value", 3, 1)),
                                            new Expr.Binary(
                                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "depth", "depth", 3,
                                                    1)),
                                                new Token(TokenType.MINUS, "-", null, 3, 1),
                                                new Expr.Literal(1.0)
                                            )
                                        }
                                    ))
                                )
                            })
                        )
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "sum", "sum", 4, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.If(
                            new Expr.Binary(
                                new Expr.Get(
                                    new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                                    new Token(TokenType.IDENTIFIER, "next", "next", 4, 1)
                                ),
                                new Token(TokenType.EQUAL_EQUAL, "==", null, 4, 1),
                                new Expr.Literal(null)
                            ),
                            new Stmt.Return(
                                new Token(TokenType.RETURN, "return", null, 4, 1),
                                new Expr.Get(
                                    new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                                    new Token(TokenType.IDENTIFIER, "value", "value", 4, 1)
                                )
                            ),
                            new Stmt.Return(
                                new Token(TokenType.RETURN, "return", null, 4, 1),
                                new Expr.Binary(
                                    new Expr.Get(
                                        new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                                        new Token(TokenType.IDENTIFIER, "value", "value", 4, 1)
                                    ),
                                    new Token(TokenType.PLUS, "+", null, 4, 1),
                                    new Expr.Call(
                                        new Expr.Get(
                                            new Expr.Get(
                                                new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                                                new Token(TokenType.IDENTIFIER, "next", "next", 4, 1)
                                            ),
                                            new Token(TokenType.IDENTIFIER, "sum", "sum", 4, 1)
                                        ),
                                        new Token(TokenType.LEFT_PAREN, "(", null, 4, 1),
                                        new List<Expr>()
                                    )
                                )
                            )
                        )
                    }
                )
            }
        );
        nodeClassStmt.Accept(interpreter);

        // Create root node
        var root = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Node", "Node", 5, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 5, 1),
            new List<Expr> { new Expr.Literal(1.0) }
        );

        // Store root in variable
        var declRoot = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", "list", 6, 1),
            root
        );
        declRoot.Accept(interpreter);

        // Append values with depth limit
        var appendValues = new List<double> { 2.0, 3.0 };
        foreach (var value in appendValues)
        {
            var appendCall = new Expr.Call(
                new Expr.Get(
                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", "list", 7, 1)),
                    new Token(TokenType.IDENTIFIER, "append", "append", 7, 1)
                ),
                new Token(TokenType.LEFT_PAREN, "(", null, 7, 1),
                new List<Expr>
                {
                    new Expr.Literal(value),
                    new Expr.Literal(3.0) // Depth limit
                }
            );
            appendCall.Accept(interpreter);
        }

        // Calculate sum
        var sumCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", "list", 8, 1)),
                new Token(TokenType.IDENTIFIER, "sum", "sum", 8, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 8, 1),
            new List<Expr>()
        );

        var result = sumCall.Accept(interpreter);
        Assert.Equal(6.0, (double)result); // 1 + 2 + 3
    }

    // Additional Edge Cases and Error Handling
    [Fact]
    public void Execute_DivisionByNonNumber_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal(10.0),
            new Token(TokenType.SLASH, "/", null, 1, 1),
            new Expr.Literal("not a number")
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_ComparisonWithInvalidTypes_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Binary(
            new Expr.Literal("string"),
            new Token(TokenType.GREATER, ">", null, 1, 1),
            new Expr.Literal(42.0)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_UnaryOperationOnInvalidType_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Unary(
            new Token(TokenType.MINUS, "-", null, 1, 1),
            new Expr.Literal("not a number")
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_MethodCallOnNonInstance_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Get(
            new Expr.Literal(42.0),
            new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_PropertySetOnNonInstance_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        var expr = new Expr.Set(
            new Expr.Literal(42.0),
            new Token(TokenType.IDENTIFIER, "property", "property", 1, 1),
            new Expr.Literal(24.0)
        );

        Assert.Throws<RuntimeError>(() => expr.Accept(interpreter));
    }

    [Fact]
    public void Execute_SuperWithoutSuperclass_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        // Declare class without superclass
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 2, 1),
                            new Expr.Super(
                                new Token(TokenType.SUPER, "super", null, 2, 1),
                                new Token(TokenType.IDENTIFIER, "method", "method", 2, 1)
                            )
                        )
                    }
                )
            }
        );
        classStmt.Accept(interpreter);

        // Create instance and call method
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 3, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "test", "test", 4, 1),
            instance
        );
        declVar.Accept(interpreter);

        var methodCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 5, 1)),
                new Token(TokenType.IDENTIFIER, "method", "method", 5, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 5, 1),
            new List<Expr>()
        );

        Assert.Throws<RuntimeError>(() => methodCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_CircularInheritance_ThrowsError()
    {
        var interpreter = new MDA.Interpreter();
        // Declare class A that inherits from B (which doesn't exist yet)
        var classAStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "A", "A", 1, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "B", "B", 1, 1)),
            new List<Stmt.Function>()
        );

        // Declare class B that inherits from A (creating a cycle)
        var classBStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "B", "B", 2, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "A", "A", 2, 1)),
            new List<Stmt.Function>()
        );

        Assert.Throws<RuntimeError>(() =>
        {
            classAStmt.Accept(interpreter);
            classBStmt.Accept(interpreter);
        });
    }

    [Fact]
    public void Execute_NestedBlockScoping()
    {
        var interpreter = new MDA.Interpreter();

        var globalVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(1.0)
        );
        globalVar.Accept(interpreter);

        // Create nested blocks with shadowing
        var stmt = new Stmt.Block(new List<Stmt>
        {
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(1.0)
            ),
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Var(
                    new Token(TokenType.IDENTIFIER, "x", "x", 2, 1),
                    new Expr.Literal(2.0)
                ),
                new Stmt.Block(new List<Stmt>
                {
                    new Stmt.Var(
                        new Token(TokenType.IDENTIFIER, "x", "x", 3, 1),
                        new Expr.Literal(3.0)
                    )
                })
            })
        });

        stmt.Accept(interpreter);

        // After all blocks, x should have its original value
        var expr = new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 4, 1));
        var result = expr.Accept(interpreter);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Execute_FunctionRecursionLimit()
    {
        var interpreter = new MDA.Interpreter();
        // Declare a function that calls itself with a counter to limit recursion
        var declStmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "limited", "limited", 1, 1),
            new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)
            },
            new List<Stmt>
            {
                new Stmt.If(
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)),
                        new Token(TokenType.LESS_EQUAL, "<=", null, 1, 1),
                        new Expr.Literal(0.0)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 1, 1),
                        new Expr.Literal(0.0)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 1, 1),
                        new Expr.Call(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "limited", "limited", 1, 1)),
                            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                            new List<Expr>
                            {
                                new Expr.Binary(
                                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 1, 1)),
                                    new Token(TokenType.MINUS, "-", null, 1, 1),
                                    new Expr.Literal(1.0)
                                )
                            }
                        )
                    )
                )
            }
        );
        declStmt.Accept(interpreter);

        // Call the function with a reasonable depth
        var callExpr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "limited", "limited", 2, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(10.0) }
        );

        var result = callExpr.Accept(interpreter);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Execute_MethodChaining()
    {
        var interpreter = new MDA.Interpreter();
        // Declare a class with chainable methods
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Chainable", "Chainable", 1, 1),
            null,
            new List<Stmt.Function>
            {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 2, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 2, 1)),
                            new Token(TokenType.IDENTIFIER, "value", "value", 2, 1),
                            new Expr.Literal(0.0)
                        ))
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "add", "add", 3, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "n", "n", 3, 1)
                    },
                    new List<Stmt>
                    {
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                            new Token(TokenType.IDENTIFIER, "value", "value", 3, 1),
                            new Expr.Binary(
                                new Expr.Get(
                                    new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                                    new Token(TokenType.IDENTIFIER, "value", "value", 3, 1)
                                ),
                                new Token(TokenType.PLUS, "+", null, 3, 1),
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 3, 1))
                            )
                        )),
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 3, 1),
                            new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1))
                        )
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "multiply", "multiply", 4, 1),
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "n", "n", 4, 1)
                    },
                    new List<Stmt>
                    {
                        new Stmt.Expression(new Expr.Set(
                            new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                            new Token(TokenType.IDENTIFIER, "value", "value", 4, 1),
                            new Expr.Binary(
                                new Expr.Get(
                                    new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                                    new Token(TokenType.IDENTIFIER, "value", "value", 4, 1)
                                ),
                                new Token(TokenType.STAR, "*", null, 4, 1),
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 4, 1))
                            )
                        )),
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 4, 1),
                            new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1))
                        )
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "getValue", "getValue", 5, 1),
                    new List<Token>(),
                    new List<Stmt>
                    {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 5, 1),
                            new Expr.Get(
                                new Expr.This(new Token(TokenType.THIS, "this", null, 5, 1)),
                                new Token(TokenType.IDENTIFIER, "value", "value", 5, 1)
                            )
                        )
                    }
                )
            }
        );
        classStmt.Accept(interpreter);

        // Create instance
        var instance = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Chainable", "Chainable", 6, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 6, 1),
            new List<Expr>()
        );

        // Store instance in variable
        var declVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "obj", "obj", 7, 1),
            instance
        );
        declVar.Accept(interpreter);

        // Chain method calls: obj.add(2).multiply(3).add(4)
        var chainedCall = new Expr.Call(
            new Expr.Get(
                new Expr.Call(
                    new Expr.Get(
                        new Expr.Call(
                            new Expr.Get(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "obj", "obj", 8, 1)),
                                new Token(TokenType.IDENTIFIER, "add", "add", 8, 1)
                            ),
                            new Token(TokenType.LEFT_PAREN, "(", null, 8, 1),
                            new List<Expr> { new Expr.Literal(2.0) }
                        ),
                        new Token(TokenType.IDENTIFIER, "multiply", "multiply", 8, 1)
                    ),
                    new Token(TokenType.LEFT_PAREN, "(", null, 8, 1),
                    new List<Expr> { new Expr.Literal(3.0) }
                ),
                new Token(TokenType.IDENTIFIER, "add", "add", 8, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 8, 1),
            new List<Expr> { new Expr.Literal(4.0) }
        );

        chainedCall.Accept(interpreter);

        // Get final value
        var getValueCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "obj", "obj", 9, 1)),
                new Token(TokenType.IDENTIFIER, "getValue", "getValue", 9, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 9, 1),
            new List<Expr>()
        );

        var result = getValueCall.Accept(interpreter);
        Assert.Equal(10.0, result); // (2 * 3) + 4 = 10
    }

    [Fact]
    public void Execute_ListCreation()
    {
        var interpreter = new MDA.Interpreter();

        // Create list and assign to variable
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Get list through variable
        var getList = new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1));
        var result = getList.Accept(interpreter);

        Assert.IsType<MdaList>(result);
        Assert.Equal("[1, 2, 3]", result.ToString());
    }

    [Fact]
    public void Execute_ListAccess()
    {
        var interpreter = new MDA.Interpreter();

        // Create list and assign to variable
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Access list element
        var access = new Expr.ListAccess(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
            new Expr.Literal(1.0),
            new Token(TokenType.LEFT_BRACKET, "[", null, 2, 1)
        );

        var result = access.Accept(interpreter);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void Execute_ListAssignment()
    {
        var interpreter = new MDA.Interpreter();

        // Create list and assign to variable
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Assign to list element
        var assign = new Expr.ListAssign(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
            new Expr.Literal(1.0),
            new Expr.Literal(5.0),
            new Token(TokenType.LEFT_BRACKET, "[", null, 2, 1)
        );

        var result = assign.Accept(interpreter);
        Assert.Equal(5.0, result);

        // Verify assignment
        var access = new Expr.ListAccess(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
            new Expr.Literal(1.0),
            new Token(TokenType.LEFT_BRACKET, "[", null, 3, 1)
        );
        Assert.Equal(5.0, access.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Push()
    {
        var interpreter = new MDA.Interpreter();

        // Create empty list
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>())
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Push values using method calls
        var pushCall1 = new Stmt.Expression(new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "push", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(1.0) }
        ));

        var pushCall2 = new Stmt.Expression(new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "push", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr> { new Expr.Literal(2.0) }
        ));

        interpreter.Interpret(new List<Stmt> { pushCall1, pushCall2 });

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 4, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 4, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 4, 1),
            new List<Expr>()
        );

        Assert.Equal("[1, 2]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Pop()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Pop value
        var popCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "pop", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr>()
        );

        var result = popCall.Accept(interpreter);
        Assert.Equal(2.0, result);

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        Assert.Equal("[1]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Length()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Get length
        var lengthCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "length", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr>()
        );

        var result = lengthCall.Accept(interpreter);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void Execute_ListMethods_Sort()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with unsorted values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(3.0),
                new Expr.Literal(1.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Sort list
        var sortCall = new Stmt.Expression(new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "sort", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr>()
        ));

        interpreter.Interpret(new List<Stmt> { sortCall });

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        Assert.Equal("[1, 2, 3]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Reverse()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Reverse list
        var reverseCall = new Stmt.Expression(new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "reverse", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr>()
        ));

        interpreter.Interpret(new List<Stmt> { reverseCall });

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        Assert.Equal("[3, 2, 1]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Contains()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Test contains with existing value
        var containsCall1 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "contains", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(2.0) }
        );

        // Test contains with non-existing value
        var containsCall2 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "contains", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr> { new Expr.Literal(3.0) }
        );

        Assert.True((bool)containsCall1.Accept(interpreter));
        Assert.False((bool)containsCall2.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_IndexOf()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Test indexOf with existing value
        var indexOfCall1 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "indexOf", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(2.0) }
        );

        // Test indexOf with non-existing value
        var indexOfCall2 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "indexOf", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr> { new Expr.Literal(3.0) }
        );

        Assert.Equal(1, indexOfCall1.Accept(interpreter));
        Assert.Equal(-1, indexOfCall2.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_LastIndexOf()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Test lastIndexOf
        var lastIndexOfCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "lastIndexOf", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(2.0) }
        );

        Assert.Equal(2, lastIndexOfCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Remove()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Remove first occurrence of 2
        var removeCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "remove", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(2.0) }
        );

        var result = removeCall.Accept(interpreter);
        Assert.True((bool)result);

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        Assert.Equal("[1, 2]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_RemoveAll()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Remove all occurrences of 2
        var removeAllCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "removeAll", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(2.0) }
        );

        var result = removeAllCall.Accept(interpreter);
        Assert.Equal(2, result);

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        Assert.Equal("[1]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_InsertAt()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(3.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Insert value at index 1
        var insertCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "insertAt", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(1.0), new Expr.Literal(2.0) }
        );

        insertCall.Accept(interpreter);

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        Assert.Equal("[1, 2, 3]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_RemoveAt()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Remove value at index 1
        var removeAtCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "removeAt", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr> { new Expr.Literal(1.0) }
        );

        removeAtCall.Accept(interpreter);

        // Verify list state
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr>()
        );

        Assert.Equal("[1, 3]", toStringCall.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Sorted()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with unsorted values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(3.0),
                new Expr.Literal(1.0),
                new Expr.Literal(2.0)
            })
        );

        interpreter.Interpret(new List<Stmt> { createList });

        // Get sorted copy
        var sortedCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 2, 1)),
                new Token(TokenType.IDENTIFIER, "sorted", null, 2, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 2, 1),
            new List<Expr>()
        );

        // Store sorted copy in new variable
        var storeSorted = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "sortedList", null, 3, 1),
            sortedCall
        );

        interpreter.Interpret(new List<Stmt> { storeSorted });

        // Verify original list is unchanged
        var toStringCall1 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 4, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 4, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 4, 1),
            new List<Expr>()
        );

        // Verify sorted list is sorted
        var toStringCall2 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "sortedList", null, 5, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 5, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 5, 1),
            new List<Expr>()
        );

        Assert.Equal("[3, 1, 2]", toStringCall1.Accept(interpreter));
        Assert.Equal("[1, 2, 3]", toStringCall2.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_Filtered()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0),
                new Expr.Literal(4.0)
            })
        );

        // Create filter function that keeps even numbers
        var createFilterFn = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "isEven", null, 2, 1),
            new List<Token> { new Token(TokenType.IDENTIFIER, "n", null, 2, 1) },
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 2, 1),
                    new Expr.Binary(
                        new Expr.Binary(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", null, 2, 1)),
                            new Token(TokenType.PERCENT, "%", null, 2, 1),
                            new Expr.Literal(2.0)
                        ),
                        new Token(TokenType.EQUAL_EQUAL, "==", null, 2, 1),
                        new Expr.Literal(0.0)
                    )
                )
            }
        );

        interpreter.Interpret(new List<Stmt> { createList, createFilterFn });

        // Get filtered copy
        var filteredCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "filtered", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr> { new Expr.Variable(new Token(TokenType.IDENTIFIER, "isEven", null, 3, 1)) }
        );

        // Store filtered copy in new variable
        var storeFiltered = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "filteredList", null, 4, 1),
            filteredCall
        );

        interpreter.Interpret(new List<Stmt> { storeFiltered });

        // Verify original list is unchanged
        var toStringCall1 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 5, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 5, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 5, 1),
            new List<Expr>()
        );

        // Verify filtered list contains only even numbers
        var toStringCall2 = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "filteredList", null, 6, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 6, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 6, 1),
            new List<Expr>()
        );

        Assert.Equal("[1, 2, 3, 4]", toStringCall1.Accept(interpreter));
        Assert.Equal("[2, 4]", toStringCall2.Accept(interpreter));
    }

    [Fact]
    public void Execute_ListMethods_CustomSort()
    {
        var interpreter = new MDA.Interpreter();

        // Create list with values
        var createList = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "list", null, 1, 1),
            new Expr.List(new List<Expr>
            {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0),
                new Expr.Literal(4.0)
            })
        );

        // Create custom sort function that sorts in descending order
        var createSortFn = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "descendingSort", null, 2, 1),
            new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "a", null, 2, 1),
                new Token(TokenType.IDENTIFIER, "b", null, 2, 1)
            },
            new List<Stmt>
            {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 2, 1),
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "b", null, 2, 1)),
                        new Token(TokenType.MINUS, "-", null, 2, 1),
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", null, 2, 1))
                    )
                )
            }
        );

        interpreter.Interpret(new List<Stmt> { createList, createSortFn });

        // Sort using custom sort function
        var customSortCall = new Stmt.Expression(new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 3, 1)),
                new Token(TokenType.IDENTIFIER, "customSort", null, 3, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 3, 1),
            new List<Expr> { new Expr.Variable(new Token(TokenType.IDENTIFIER, "descendingSort", null, 3, 1)) }
        ));

        interpreter.Interpret(new List<Stmt> { customSortCall });

        // Verify list is sorted in descending order
        var toStringCall = new Expr.Call(
            new Expr.Get(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", null, 4, 1)),
                new Token(TokenType.IDENTIFIER, "toString", null, 4, 1)
            ),
            new Token(TokenType.LEFT_PAREN, "(", null, 4, 1),
            new List<Expr>()
        );

        Assert.Equal("[4, 3, 2, 1]", toStringCall.Accept(interpreter));
    }

    // Error handling
    [Fact]
    public void Execute_ThrowError()
    {
        var interpreter = new MDA.Interpreter();

        var throwCall = new Stmt.Throw(
            new Token(TokenType.THROW, "throw", null, 1, 1),
            new Expr.Call(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                new List<Expr>
                {
                    new Expr.Literal("Error message")
                }
            )
        );

        interpreter.Interpret(new List<Stmt> { throwCall });

        // The interpreter should report an error for the thrown error
        Assert.Equal("Error message", _errorReporter.LastErrorMessage);
    }

    [Fact]
    public void Execute_CatchError()
    {
        var interpreter = new MDA.Interpreter();

        var throwCall = new Stmt.Throw(
            new Token(TokenType.THROW, "throw", null, 1, 1),
            new Expr.Call(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                new List<Expr>
                {
                    new Expr.Literal("Error message")
                }
            )
        );

        var tryCall = new Stmt.Try(
            new Stmt.Block(new List<Stmt> { throwCall }),
            new CatchClause(
                new Token(TokenType.IDENTIFIER, "error", "error", 1, 1),
                new Stmt.Block(
                    new List<Stmt> { }
                )
            ),
            null
        );

        interpreter.Interpret(new List<Stmt> { tryCall });

        // The interpreter should not report an error if the error is caught
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Execute_FinallyBlock_WhenNoErrorIsThrown()
    {
        var interpreter = new MDA.Interpreter();

        var tryCall = new Stmt.Try(
            new Stmt.Block(new List<Stmt> { }),
            null,
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Throw(
                    new Token(TokenType.THROW, "throw", null, 1, 1),
                    new Expr.Call(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                        new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                        new List<Expr>
                        {
                            new Expr.Literal("Error message from finally block")
                        }
                    )
                )
            })
        );

        interpreter.Interpret(new List<Stmt> { tryCall });

        // The error is reported from the finally block
        Assert.Equal(_errorReporter.LastErrorMessage, "Error message from finally block");
    }

    [Fact]
    public void Execute_FinallyBlock_WhenErrorIsThrown()
    {
        var interpreter = new MDA.Interpreter();

        var throwCall = new Stmt.Throw(
            new Token(TokenType.THROW, "throw", null, 1, 1),
            new Expr.Call(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                new List<Expr>
                {
                    new Expr.Literal("Error message from throw block")
                }
            )
        );

        var tryCall = new Stmt.Try(
            new Stmt.Block(new List<Stmt> { throwCall }),
            null,
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Throw(
                    new Token(TokenType.THROW, "throw", null, 1, 1),
                    new Expr.Call(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                        new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                        new List<Expr>
                        {
                            new Expr.Literal("Error message from finally block")
                        }
                    )
                )
            })
        );

        interpreter.Interpret(new List<Stmt> { tryCall });

        // The error is reported from the finally block
        Assert.Equal(_errorReporter.LastErrorMessage, "Error message from finally block");
    }

    [Fact]
    public void Execute_Code_AfterErrorIsCaught()
    {
        var interpreter = new MDA.Interpreter();

        // define a global variable
        var varDecl = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
            new Expr.Literal(1.0)
        );

        varDecl.Accept(interpreter);

        // define throw statement
        var throwCall = new Stmt.Throw(
            new Token(TokenType.THROW, "throw", null, 1, 1),
            new Expr.Call(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                new List<Expr>
                {
                    new Expr.Literal("Error message")
                }
            )
        );

        // define try statement
        var tryCall = new Stmt.Try(
            new Stmt.Block(new List<Stmt> { throwCall }),
            new CatchClause(
                new Token(TokenType.IDENTIFIER, "error", "error", 1, 1),
                new Stmt.Block(
                    new List<Stmt> { }
                )
            ),
            null
        );

        // define a code block with try statement and an assign expression after
        var codeBlock = new Stmt.Block(new List<Stmt>
        {
            tryCall,
            new Stmt.Expression(
                new Expr.Assign(
                    new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
                    new Expr.Literal(2.0)
                )
            )
        });

        interpreter.Interpret(new List<Stmt> { codeBlock });

        var variable = new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1));
        var result = variable.Accept(interpreter);

        // The assign statement should have been executed after the error was caught
        Assert.Equal((double)result, 2.0);
    }

    [Fact]
    public void Execute_UncaughtError_StopsExecution()
    {
        var interpreter = new MDA.Interpreter();

        // Declare variable a = 1
        var varDecl = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
            new Expr.Literal(1.0)
        );
        varDecl.Accept(interpreter);

        // Throw error without try-catch
        var throwStmt = new Stmt.Throw(
            new Token(TokenType.THROW, "throw", null, 1, 1),
            new Expr.Call(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                new List<Expr> { new Expr.Literal("Uncaught error") }
            )
        );

        // Assignment after throw (shouldn't execute)
        var assignStmt = new Stmt.Expression(
            new Expr.Assign(
                new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
                new Expr.Literal(2.0)
            )
        );

        var block = new Stmt.Block(new List<Stmt> { throwStmt, assignStmt });
        interpreter.Interpret(new List<Stmt> { block });

        // Verify execution stopped and error reported
        var variable = new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 1, 1));
        Assert.Equal(1.0, variable.Accept(interpreter));
        Assert.Equal("Uncaught error", _errorReporter.LastErrorMessage);
    }

    [Fact]
    public void Execute_NestedTryCatch_OuterCatchesError()
    {
        var interpreter = new MDA.Interpreter();

        // Inner try with uncaught error
        var innerThrow = new Stmt.Throw(
            new Token(TokenType.THROW, "throw", null, 1, 1),
            new Expr.Call(
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                new List<Expr> { new Expr.Literal("Inner error") }
            )
        );

        var outerTry = new Stmt.Try(
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Try(
                    new Stmt.Block(new List<Stmt> { innerThrow }),
                    null,
                    null
                )
            }),
            new CatchClause(
                new Token(TokenType.IDENTIFIER, "e", "e", 1, 1),
                new Stmt.Block(new List<Stmt>())
            ),
            null
        );

        interpreter.Interpret(new List<Stmt> { outerTry });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Execute_ErrorInCatchBlock_PropagatesUncaught()
    {
        var interpreter = new MDA.Interpreter();

        var tryStmt = new Stmt.Try(
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Throw(
                    new Token(TokenType.THROW, "throw", null, 1, 1),
                    new Expr.Call(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                        new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                        new List<Expr> { new Expr.Literal("Original error") }
                    )
                )
            }),
            new CatchClause(
                new Token(TokenType.IDENTIFIER, "e", "e", 1, 1),
                new Stmt.Block(new List<Stmt>
                {
                    new Stmt.Throw(
                        new Token(TokenType.THROW, "throw", null, 1, 1),
                        new Expr.Call(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                            new List<Expr> { new Expr.Literal("Catch error") }
                        )
                    )
                })
            ),
            null
        );

        interpreter.Interpret(new List<Stmt> { tryStmt });
        Assert.Equal("Catch error", _errorReporter.LastErrorMessage);
        Assert.True(_errorReporter.HadRuntimeError);
    }

    [Fact]
    public void Execute_FinallyBlock_ExecutesAfterNormalExit()
    {
        var interpreter = new MDA.Interpreter();

        // Track finally execution
        var varDecl = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "executed", "executed", 1, 1),
            new Expr.Literal(false)
        );
        varDecl.Accept(interpreter);

        var tryStmt = new Stmt.Try(
            new Stmt.Block(new List<Stmt>()),
            null,
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Expression(
                    new Expr.Assign(
                        new Token(TokenType.IDENTIFIER, "executed", "executed", 1, 1),
                        new Expr.Literal(true)
                    )
                )
            })
        );

        interpreter.Interpret(new List<Stmt> { tryStmt });
        var variable = new Expr.Variable(new Token(TokenType.IDENTIFIER, "executed", "executed", 1, 1));
        Assert.True((bool)variable.Accept(interpreter));
        Assert.False(_errorReporter.HadRuntimeError);
    }

    [Fact]
    public void Execute_CatchClause_AccessErrorMessage()
    {
        var interpreter = new MDA.Interpreter();

        // Setup error message capture
        var varDecl = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "msg", "msg", 1, 1),
            new Expr.Literal("")
        );
        varDecl.Accept(interpreter);

        var tryStmt = new Stmt.Try(
            new Stmt.Block(new List<Stmt>
            {
                new Stmt.Throw(
                    new Token(TokenType.THROW, "throw", null, 1, 1),
                    new Expr.Call(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "Error", null, 1, 1)),
                        new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
                        new List<Expr> { new Expr.Literal("Test message") }
                    )
                )
            }),
            new CatchClause(
                new Token(TokenType.IDENTIFIER, "e", "e", 1, 1),
                new Stmt.Block(new List<Stmt>
                {
                    new Stmt.Expression(
                        new Expr.Assign(
                            new Token(TokenType.IDENTIFIER, "msg", "msg", 1, 1),
                            new Expr.Get(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "e", "e", 1, 1)),
                                new Token(TokenType.IDENTIFIER, "message", "message", 1, 1)
                            )
                        )
                    )
                })
            ),
            null
        );

        interpreter.Interpret(new List<Stmt> { tryStmt });
        var variable = new Expr.Variable(new Token(TokenType.IDENTIFIER, "msg", "msg", 1, 1));
        Assert.Equal("Test message", variable.Accept(interpreter));
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Execute_ThrowNonErrorObject_ReportsError()
    {
        var interpreter = new MDA.Interpreter();

        interpreter.Interpret(new List<Stmt>
        {
            new Stmt.Throw(
                new Token(TokenType.THROW, "throw", null, 1, 1),
                new Expr.Literal(42) // Throw primitive value
            )
        });

        Assert.True(_errorReporter.HadRuntimeError);
        Assert.Equal("Can only throw Error instances.", _errorReporter.LastErrorMessage);
    }
}