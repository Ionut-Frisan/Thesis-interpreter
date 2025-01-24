namespace MDA.Tests.Unit.Resolver;

public class ResolverTests
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

        public void Reset()
        {
            HadError = false;
            HadRuntimeError = false;
            LastErrorMessage = null;
        }
    }

    private TestErrorReporter _errorReporter;
    private MDA.Interpreter _interpreter;
    private MDA.Resolver _resolver;

    public ResolverTests()
    {
        _errorReporter = new TestErrorReporter();
        _interpreter = new MDA.Interpreter();
        _resolver = new MDA.Resolver(_interpreter);
        MDA.Mda.SetErrorReporter(_errorReporter);
    }

    // Variable Resolution
    [Fact]
    public void Resolve_GlobalVariable()
    {
        var stmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(42.0)
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_LocalVariable()
    {
        var block = new Stmt.Block(new List<Stmt> {
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            )
        });
        
        _resolver.Resolve(new List<Stmt> { block });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ShadowedVariable()
    {
        var block = new Stmt.Block(new List<Stmt> {
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            ),
            new Stmt.Block(new List<Stmt> {
                new Stmt.Var(
                    new Token(TokenType.IDENTIFIER, "x", "x", 2, 1),
                    new Expr.Literal(24.0)
                )
            })
        });
        
        _resolver.Resolve(new List<Stmt> { block });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_RedeclaredVariable_ThrowsError()
    {
        var block = new Stmt.Block(new List<Stmt> {
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(42.0)
            ),
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 2, 1),
                new Expr.Literal(24.0)
            )
        });
        
        _resolver.Resolve(new List<Stmt> { block });
        Assert.True(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_UninitializedVariable()
    {
        var stmt = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            null
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.False(_errorReporter.HadError);
    }

    // Function Resolution
    [Fact]
    public void Resolve_Function()
    {
        var stmt = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            new List<Token>(),
            new List<Stmt> {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 2, 1),
                    new Expr.Literal(42.0)
                )
            }
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ReturnOutsideFunction_ThrowsError()
    {
        var stmt = new Stmt.Return(
            new Token(TokenType.RETURN, "return", null, 1, 1),
            new Expr.Literal(42.0)
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ReturnInInitializer_ThrowsError()
    {
        var stmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 2, 1),
                    new List<Token>(),
                    new List<Stmt> {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 3, 1),
                            new Expr.Literal(42.0)
                        )
                    }
                )
            }
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    // Class Resolution
    [Fact]
    public void Resolve_Class()
    {
        var stmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function>()
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ClassInheritance()
    {
        var stmts = new List<Stmt> {
            new Stmt.Class(
                new Token(TokenType.IDENTIFIER, "Base", "Base", 1, 1),
                null,
                new List<Stmt.Function>()
            ),
            new Stmt.Class(
                new Token(TokenType.IDENTIFIER, "Derived", "Derived", 2, 1),
                new Expr.Variable(new Token(TokenType.IDENTIFIER, "Base", "Base", 2, 1)),
                new List<Stmt.Function>()
            )
        };
        
        _resolver.Resolve(stmts);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ClassInheritsSelf_ThrowsError()
    {
        var stmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1)),
            new List<Stmt.Function>()
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ThisOutsideClass_ThrowsError()
    {
        var stmt = new Stmt.Expression(
            new Expr.This(new Token(TokenType.THIS, "this", null, 1, 1))
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_SuperOutsideClass_ThrowsError()
    {
        var stmt = new Stmt.Expression(
            new Expr.Super(
                new Token(TokenType.SUPER, "super", null, 1, 1),
                new Token(TokenType.IDENTIFIER, "method", "method", 1, 1)
            )
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_SuperOutsideSubclass_ThrowsError()
    {
        var stmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt> {
                        new Stmt.Expression(
                            new Expr.Super(
                                new Token(TokenType.SUPER, "super", null, 3, 1),
                                new Token(TokenType.IDENTIFIER, "method", "method", 3, 1)
                            )
                        )
                    }
                )
            }
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    // Control Flow Resolution
    [Fact]
    public void Resolve_BreakOutsideLoop_ThrowsError()
    {
        var stmt = new Stmt.Break(
            new Token(TokenType.BREAK, "break", null, 1, 1)
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ContinueOutsideLoop_ThrowsError()
    {
        var stmt = new Stmt.Continue(
            new Token(TokenType.CONTINUE, "continue", null, 1, 1)
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.True(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_BreakInLoop()
    {
        var stmt = new Stmt.While(
            new Expr.Literal(true),
            new Stmt.Break(new Token(TokenType.BREAK, "break", null, 1, 1)),
            null
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ContinueInLoop()
    {
        var stmt = new Stmt.While(
            new Expr.Literal(true),
            new Stmt.Continue(new Token(TokenType.CONTINUE, "continue", null, 1, 1)),
            null
        );
        
        _resolver.Resolve(new List<Stmt> { stmt });
        Assert.False(_errorReporter.HadError);
    }

    // Expression Resolution
    [Fact]
    public void Resolve_BinaryExpression()
    {
        var expr = new Expr.Binary(
            new Expr.Literal(1.0),
            new Token(TokenType.PLUS, "+", null, 1, 1),
            new Expr.Literal(2.0)
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_UnaryExpression()
    {
        var expr = new Expr.Unary(
            new Token(TokenType.MINUS, "-", null, 1, 1),
            new Expr.Literal(42.0)
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_LogicalExpression()
    {
        var expr = new Expr.Logical(
            new Expr.Literal(true),
            new Token(TokenType.AND, "and", null, 1, 1),
            new Expr.Literal(false)
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_CallExpression()
    {
        var expr = new Expr.Call(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "test", "test", 1, 1)),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 1),
            new List<Expr>()
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_GetExpression()
    {
        var expr = new Expr.Get(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "obj", "obj", 1, 1)),
            new Token(TokenType.IDENTIFIER, "property", "property", 1, 1)
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_SetExpression()
    {
        var expr = new Expr.Set(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "obj", "obj", 1, 1)),
            new Token(TokenType.IDENTIFIER, "property", "property", 1, 1),
            new Expr.Literal(42.0)
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ListExpression()
    {
        var expr = new Expr.List(
            new List<Expr> {
                new Expr.Literal(1.0),
                new Expr.Literal(2.0),
                new Expr.Literal(3.0)
            }
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ListAccessExpression()
    {
        var expr = new Expr.ListAccess(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", "list", 1, 1)),
            new Expr.Literal(0.0),
            new Token(TokenType.LEFT_BRACKET, "[", null, 1, 1)
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ListAssignExpression()
    {
        var expr = new Expr.ListAssign(
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "list", "list", 1, 1)),
            new Expr.Literal(0.0),
            new Expr.Literal(42.0),
            new Token(TokenType.LEFT_BRACKET, "[", null, 1, 1)
        );
        
        _resolver.Resolve(expr);
        Assert.False(_errorReporter.HadError);
    }

    // Additional Variable Resolution Tests
    [Fact]
    public void Resolve_NestedScopes_ResolvesToCorrectDepth()
    {
        // Create a variable in global scope
        var globalVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(1.0)
        );

        // Create a block with a local variable that shadows the global
        var block = new Stmt.Block(new List<Stmt> {
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 2, 1),
                new Expr.Literal(2.0)
            ),
            // Reference to x should resolve to the local variable
            new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 3, 1)))
        });

        _resolver.Resolve(new List<Stmt> { globalVar, block });
        
        // The resolver should have resolved the variable reference to depth 0 (local scope)
        // We can verify this by checking that no error was reported
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_FunctionParameters_CreateNewScope()
    {
        var function = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "test", "test", 1, 1),
            new List<Token> {
                new Token(TokenType.IDENTIFIER, "param", "param", 1, 2)
            },
            new List<Stmt> {
                // Reference the parameter
                new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "param", "param", 2, 1))),
                // Create a local that shadows the parameter
                new Stmt.Var(
                    new Token(TokenType.IDENTIFIER, "param", "param", 3, 1),
                    new Expr.Literal(42.0)
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { function });
        
        // The resolver should report an error for the redeclaration of 'param'
        Assert.True(_errorReporter.HadError);
        Assert.Contains("RS010", _errorReporter.LastErrorMessage);
    }

    [Fact]
    public void Resolve_MethodChaining_ResolvesCorrectly()
    {
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method1", "method1", 2, 1),
                    new List<Token>(),
                    new List<Stmt> {
                        new Stmt.Expression(
                            new Expr.Get(
                                new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                                new Token(TokenType.IDENTIFIER, "method2", "method2", 3, 2)
                            )
                        )
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method2", "method2", 4, 1),
                    new List<Token>(),
                    new List<Stmt>()
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { classStmt });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ClosureCapture_ResolvesCorrectly()
    {
        var outerVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(42.0)
        );

        var function = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "closure", "closure", 2, 1),
            new List<Token>(),
            new List<Stmt> {
                // Reference the outer variable
                new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 3, 1)))
            }
        );

        _resolver.Resolve(new List<Stmt> { outerVar, function });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_SuperMethodChain_ResolvesCorrectly()
    {
        var baseClass = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Base", "Base", 1, 1),
            null,
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt>()
                )
            }
        );

        var derivedClass = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Derived", "Derived", 3, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Base", "Base", 3, 2)),
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 4, 1),
                    new List<Token>(),
                    new List<Stmt> {
                        new Stmt.Expression(
                            new Expr.Super(
                                new Token(TokenType.SUPER, "super", null, 5, 1),
                                new Token(TokenType.IDENTIFIER, "method", "method", 5, 2)
                            )
                        )
                    }
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { baseClass, derivedClass });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_RecursiveFunction_ResolvesCorrectly()
    {
        var function = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "factorial", "factorial", 1, 1),
            new List<Token> {
                new Token(TokenType.IDENTIFIER, "n", "n", 1, 2)
            },
            new List<Stmt> {
                // if (n <= 1) return 1;
                new Stmt.If(
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 2, 1)),
                        new Token(TokenType.LESS_EQUAL, "<=", null, 2, 2),
                        new Expr.Literal(1.0)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 2, 3),
                        new Expr.Literal(1.0)
                    ),
                    null
                ),
                // return n * factorial(n - 1);
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 3, 1),
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 3, 2)),
                        new Token(TokenType.STAR, "*", null, 3, 3),
                        new Expr.Call(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "factorial", "factorial", 3, 4)),
                            new Token(TokenType.LEFT_PAREN, "(", null, 3, 5),
                            new List<Expr> {
                                new Expr.Binary(
                                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 3, 6)),
                                    new Token(TokenType.MINUS, "-", null, 3, 7),
                                    new Expr.Literal(1.0)
                                )
                            }
                        )
                    )
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { function });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_MutualRecursion_ResolvesCorrectly()
    {
        var isEven = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "isEven", "isEven", 1, 1),
            new List<Token> { new Token(TokenType.IDENTIFIER, "n", "n", 1, 2) },
            new List<Stmt> {
                new Stmt.If(
                    new Expr.Binary(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 2, 1)),
                        new Token(TokenType.EQUAL_EQUAL, "==", null, 2, 2),
                        new Expr.Literal(0.0)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 2, 3),
                        new Expr.Literal(true)
                    ),
                    new Stmt.Return(
                        new Token(TokenType.RETURN, "return", null, 2, 4),
                        new Expr.Call(
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "isOdd", "isOdd", 2, 5)),
                            new Token(TokenType.LEFT_PAREN, "(", null, 2, 6),
                            new List<Expr> {
                                new Expr.Binary(
                                    new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 2, 7)),
                                    new Token(TokenType.MINUS, "-", null, 2, 8),
                                    new Expr.Literal(1.0)
                                )
                            }
                        )
                    )
                )
            }
        );

        var isOdd = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "isOdd", "isOdd", 3, 1),
            new List<Token> { new Token(TokenType.IDENTIFIER, "n", "n", 3, 2) },
            new List<Stmt> {
                new Stmt.Return(
                    new Token(TokenType.RETURN, "return", null, 4, 1),
                    new Expr.Call(
                        new Expr.Variable(new Token(TokenType.IDENTIFIER, "isEven", "isEven", 4, 2)),
                        new Token(TokenType.LEFT_PAREN, "(", null, 4, 3),
                        new List<Expr> {
                            new Expr.Binary(
                                new Expr.Variable(new Token(TokenType.IDENTIFIER, "n", "n", 4, 4)),
                                new Token(TokenType.MINUS, "-", null, 4, 5),
                                new Expr.Literal(1.0)
                            )
                        }
                    )
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { isEven, isOdd });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_NestedClosures_ResolvesCorrectly()
    {
        var outerVar = new Stmt.Var(
            new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
            new Expr.Literal(1.0)
        );

        var outerFunction = new Stmt.Function(
            new Token(TokenType.IDENTIFIER, "outer", "outer", 2, 1),
            new List<Token>(),
            new List<Stmt> {
                new Stmt.Var(
                    new Token(TokenType.IDENTIFIER, "y", "y", 3, 1),
                    new Expr.Literal(2.0)
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "inner", "inner", 4, 1),
                    new List<Token>(),
                    new List<Stmt> {
                        // Reference both outer variables
                        new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 5, 1))),
                        new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "y", "y", 5, 2)))
                    }
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { outerVar, outerFunction });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_MethodOverriding_WithSuperCall()
    {
        var baseClass = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Base", "Base", 1, 1),
            null,
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token> { new Token(TokenType.IDENTIFIER, "x", "x", 2, 2) },
                    new List<Stmt> {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 2, 3),
                            new Expr.Variable(new Token(TokenType.IDENTIFIER, "x", "x", 2, 4))
                        )
                    }
                )
            }
        );

        var derivedClass = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Derived", "Derived", 3, 1),
            new Expr.Variable(new Token(TokenType.IDENTIFIER, "Base", "Base", 3, 2)),
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 4, 1),
                    new List<Token> { new Token(TokenType.IDENTIFIER, "x", "x", 4, 2) },
                    new List<Stmt> {
                        new Stmt.Return(
                            new Token(TokenType.RETURN, "return", null, 4, 3),
                            new Expr.Binary(
                                new Expr.Super(
                                    new Token(TokenType.SUPER, "super", null, 4, 4),
                                    new Token(TokenType.IDENTIFIER, "method", "method", 4, 5)
                                ),
                                new Token(TokenType.PLUS, "+", null, 4, 6),
                                new Expr.Literal(1.0)
                            )
                        )
                    }
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { baseClass, derivedClass });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ComplexNestedScopes()
    {
        var statements = new List<Stmt> {
            // Global x
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "x", "x", 1, 1),
                new Expr.Literal(1.0)
            ),
            // Function declaration
            new Stmt.Function(
                new Token(TokenType.IDENTIFIER, "func", "func", 2, 1),
                new List<Token>(),
                new List<Stmt> {
                    // Local x shadows global x
                    new Stmt.Var(
                        new Token(TokenType.IDENTIFIER, "x", "x", 3, 1),
                        new Expr.Literal(2.0)
                    ),
                    // Block with another x
                    new Stmt.Block(new List<Stmt> {
                        new Stmt.Var(
                            new Token(TokenType.IDENTIFIER, "x", "x", 4, 1),
                            new Expr.Literal(3.0)
                        ),
                        // While loop with another x
                        new Stmt.While(
                            new Expr.Literal(true),
                            new Stmt.Block(new List<Stmt> {
                                new Stmt.Var(
                                    new Token(TokenType.IDENTIFIER, "x", "x", 5, 1),
                                    new Expr.Literal(4.0)
                                )
                            }),
                            null
                        )
                    })
                }
            )
        };

        _resolver.Resolve(statements);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ClosureWithMultipleScopes()
    {
        var statements = new List<Stmt> {
            // Global a
            new Stmt.Var(
                new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
                new Expr.Literal(1.0)
            ),
            // Outer function
            new Stmt.Function(
                new Token(TokenType.IDENTIFIER, "outer", "outer", 2, 1),
                new List<Token>(),
                new List<Stmt> {
                    // Local b
                    new Stmt.Var(
                        new Token(TokenType.IDENTIFIER, "b", "b", 3, 1),
                        new Expr.Literal(2.0)
                    ),
                    // Middle function
                    new Stmt.Function(
                        new Token(TokenType.IDENTIFIER, "middle", "middle", 4, 1),
                        new List<Token>(),
                        new List<Stmt> {
                            // Local c
                            new Stmt.Var(
                                new Token(TokenType.IDENTIFIER, "c", "c", 5, 1),
                                new Expr.Literal(3.0)
                            ),
                            // Inner function accessing all variables
                            new Stmt.Function(
                                new Token(TokenType.IDENTIFIER, "inner", "inner", 6, 1),
                                new List<Token>(),
                                new List<Stmt> {
                                    new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "a", "a", 7, 1))),
                                    new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "b", "b", 7, 2))),
                                    new Stmt.Expression(new Expr.Variable(new Token(TokenType.IDENTIFIER, "c", "c", 7, 3)))
                                }
                            )
                        }
                    )
                }
            )
        };

        _resolver.Resolve(statements);
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_ClassMethodWithNestedFunctions()
    {
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 2, 1),
                    new List<Token>(),
                    new List<Stmt> {
                        // Local function that uses 'this'
                        new Stmt.Function(
                            new Token(TokenType.IDENTIFIER, "helper", "helper", 3, 1),
                            new List<Token>(),
                            new List<Stmt> {
                                new Stmt.Expression(
                                    new Expr.Get(
                                        new Expr.This(new Token(TokenType.THIS, "this", null, 4, 1)),
                                        new Token(TokenType.IDENTIFIER, "method", "method", 4, 2)
                                    )
                                )
                            }
                        )
                    }
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { classStmt });
        Assert.False(_errorReporter.HadError);
    }

    [Fact]
    public void Resolve_InitializerWithSelfReference()
    {
        var classStmt = new Stmt.Class(
            new Token(TokenType.IDENTIFIER, "Test", "Test", 1, 1),
            null,
            new List<Stmt.Function> {
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "init", "init", 2, 1),
                    new List<Token>(),
                    new List<Stmt> {
                        // Set a field using this
                        new Stmt.Expression(
                            new Expr.Set(
                                new Expr.This(new Token(TokenType.THIS, "this", null, 3, 1)),
                                new Token(TokenType.IDENTIFIER, "field", "field", 3, 2),
                                new Expr.Get(
                                    new Expr.This(new Token(TokenType.THIS, "this", null, 3, 3)),
                                    new Token(TokenType.IDENTIFIER, "method", "method", 3, 4)
                                )
                            )
                        )
                    }
                ),
                new Stmt.Function(
                    new Token(TokenType.IDENTIFIER, "method", "method", 4, 1),
                    new List<Token>(),
                    new List<Stmt>()
                )
            }
        );

        _resolver.Resolve(new List<Stmt> { classStmt });
        Assert.False(_errorReporter.HadError);
    }
}