using Moq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace MDA.Tests.Unit.Parser;

public class StatementParsingTests
{
    private readonly Mock<IErrorReporter> _mockErrorReporter = new();
    private readonly Mock<IExitHandler> _mockExitHandler = new();
    private readonly ITestOutputHelper _output;
    
    public StatementParsingTests(ITestOutputHelper output)
    {
        Mda.SetErrorReporter(_mockErrorReporter.Object);
        Mda.SetExitHandler(_mockExitHandler.Object);
        
        _output = output;
    }
    
    // Variable declaration and assignment
    [Fact]
    public void ParseVariableDeclaration_WithoutInitializer()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.VAR, "var", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 2),
            new Token(TokenType.SEMICOLON, ";", null, 1, 3),
            new Token(TokenType.EOF, "", null, 1, 4)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(var a)", result);
    }
    
    [Fact]
    public void ParseVariableDeclaration_WithInitializer()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.VAR, "var", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 2),
            new Token(TokenType.EQUAL, "=", null, 1, 3),
            new Token(TokenType.NUMBER, "1", 1, 1, 4),
            new Token(TokenType.SEMICOLON, ";", null, 1, 5),
            new Token(TokenType.EOF, "", null, 1, 6)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(var a = 1)", result);
    }
    
    [Fact]
    public void ParseAssignment_SimpleAssignment()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
            new Token(TokenType.EQUAL, "=", null, 1, 2),
            new Token(TokenType.NUMBER, "1", 1, 1, 3),
            new Token(TokenType.SEMICOLON, ";", null, 1, 4),
            new Token(TokenType.EOF, "", null, 1, 5)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(; (assign a 1))", result);
    }

    [Theory]
    [ClassData(typeof(StatementParsingData.ParseInvalidAssignmentTargetData))]
    public void ParseAssignment_ToInvalidTarget_ReportsError(List<Token> tokens)
    {
        var parser = new MDA.Parser(tokens);
        var statements = parser.Parse();
        
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(x => x.Contains("PS007"))), Times.Once());
    }
    
    [Theory]
    [ClassData(typeof(StatementParsingData.ParseCompoundAssignmentData))]
    public void ParseCompoundAssignment(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(StatementParsingData.ParseInvalidCompoundAssignmentTargetData))]
    public void ParseCompoundAssignment_WithInvalidTarget_ReportsError(List<Token> tokens)
    {
        var parser = new MDA.Parser(tokens);
        var statements = parser.Parse();
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(x => x.Contains("PS008"))), Times.Once());
    }

    // Control flow
    [Fact]
    public void ParseIf_WithoutElse()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.IF, "if", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.TRUE, "true", true, 1, 3),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 4),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 5),
            new Token(TokenType.EQUAL, "=", null, 1, 6),
            new Token(TokenType.NUMBER, "1", 1, 1, 7),
            new Token(TokenType.SEMICOLON, ";", null, 1, 8),
            new Token(TokenType.EOF, "", null, 1, 9)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(if True (; (assign a 1)))", result);
    }
    
    [Fact]
    public void ParseIf_WithElse()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.IF, "if", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.TRUE, "true", true, 1, 3),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 4),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 5),
            new Token(TokenType.EQUAL, "=", null, 1, 6),
            new Token(TokenType.NUMBER, "1", 1, 1, 7),
            new Token(TokenType.SEMICOLON, ";", null, 1, 8),
            new Token(TokenType.ELSE, "else", null, 1, 9),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 10),
            new Token(TokenType.EQUAL, "=", null, 1, 11),
            new Token(TokenType.NUMBER, "2", 2, 1, 12),
            new Token(TokenType.SEMICOLON, ";", null, 1, 13),
            new Token(TokenType.EOF, "", null, 1, 14)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(if True (; (assign a 1)) else (; (assign a 2)))", result);
    }
    
    [Fact]
    public void ParseWhile_SimpleCondition()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.WHILE, "while", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.TRUE, "true", true, 1, 3),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 4),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 5),
            new Token(TokenType.EQUAL, "=", null, 1, 6),
            new Token(TokenType.NUMBER, "1", 1, 1, 7),
            new Token(TokenType.SEMICOLON, ";", null, 1, 8),
            new Token(TokenType.EOF, "", null, 1, 9)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(while True (; (assign a 1)))", result);
    }

    [Fact]
    public void ParseWhile_ComplexBody()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.WHILE, "while", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.TRUE, "true", true, 1, 3),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 4),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 5),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 6),
            new Token(TokenType.EQUAL, "=", null, 1, 7),
            new Token(TokenType.NUMBER, "1", 1, 1, 8),
            new Token(TokenType.SEMICOLON, ";", null, 1, 9),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 10),
            new Token(TokenType.EOF, "", null, 1, 11)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(while True { (; (assign a 1))})", result);
    }
    
    [Fact]
    public void ParseFor_MissingInitializer()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FOR, "for", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.SEMICOLON, ";", null, 1, 3),
            new Token(TokenType.TRUE, "true", true, 1, 4),
            new Token(TokenType.SEMICOLON, ";", null, 1, 5),
            new Token(TokenType.TRUE, "true", true, 1, 6),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 7),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 8),
            new Token(TokenType.EQUAL, "=", null, 1, 9),
            new Token(TokenType.NUMBER, "1", 1, 1, 10),
            new Token(TokenType.SEMICOLON, ";", null, 1, 11),
            new Token(TokenType.EOF, "", null, 1, 12)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        // for is getting parsed as a while loop
        Assert.Equal("(while True { (; (assign a 1)) (; True)})", result);
    }
    
    [Fact]
    public void ParseFor_MissingCondition()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FOR, "for", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 3),
            new Token(TokenType.EQUAL, "=", null, 1, 4),
            new Token(TokenType.NUMBER, "1", 1, 1, 5),
            new Token(TokenType.SEMICOLON, ";", null, 1, 6),
            new Token(TokenType.SEMICOLON, ";", null, 1, 7),
            new Token(TokenType.TRUE, "true", true, 1, 8),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 9),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 10),
            new Token(TokenType.EQUAL, "=", null, 1, 11),
            new Token(TokenType.NUMBER, "2", 2, 1, 12),
            new Token(TokenType.SEMICOLON, ";", null, 1, 13),
            new Token(TokenType.EOF, "", null, 1, 14)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        // since for is parsed as a while loop, the assignment is not part of the loop but before it
        Assert.Equal("{ (; (assign a 1)) (while True { (; (assign a 2)) (; True)})}", result);
    }
    
    [Fact]
    public void ParseFor_MissingIncrement()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FOR, "for", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 3),
            new Token(TokenType.EQUAL, "=", null, 1, 4),
            new Token(TokenType.NUMBER, "1", 1, 1, 5),
            new Token(TokenType.SEMICOLON, ";", null, 1, 6),
            new Token(TokenType.TRUE, "true", true, 1, 7),
            new Token(TokenType.SEMICOLON, ";", null, 1, 8),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 9),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 10),
            new Token(TokenType.EQUAL, "=", null, 1, 11),
            new Token(TokenType.NUMBER, "2", 2, 1, 12),
            new Token(TokenType.SEMICOLON, ";", null, 1, 13),
            new Token(TokenType.EOF, "", null, 1, 14)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        // since for is parsed as a while loop, the assignment is not part of the loop but before it
        Assert.Equal("{ (; (assign a 1)) (while True (; (assign a 2)))}", result);
    }

    [Fact]
    public void ParseFor_AllComponents()
    {
        // for (var i = 0; i < 10; i = i + 1) { a = a + 1; }
        var tokens = new List<Token>
        {
            new Token(TokenType.FOR, "for", null, 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.VAR, "var", null, 1, 3),
            new Token(TokenType.IDENTIFIER, "i", "i", 1, 4),
            new Token(TokenType.EQUAL, "=", null, 1, 5),
            new Token(TokenType.NUMBER, "0", 0, 1, 6),
            new Token(TokenType.SEMICOLON, ";", null, 1, 7),
            new Token(TokenType.IDENTIFIER, "i", "i", 1, 8),
            new Token(TokenType.LESS, "<", null, 1, 9),
            new Token(TokenType.NUMBER, "10", 10, 1, 10),
            new Token(TokenType.SEMICOLON, ";", null, 1, 11),
            new Token(TokenType.IDENTIFIER, "i", "i", 1, 12),
            new Token(TokenType.EQUAL, "=", null, 1, 13),
            new Token(TokenType.IDENTIFIER, "i", "i", 1, 14),
            new Token(TokenType.PLUS, "+", null, 1, 15),
            new Token(TokenType.NUMBER, "1", 1, 1, 16),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 17),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 18),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 19),
            new Token(TokenType.EQUAL, "=", null, 1, 20),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 21),
            new Token(TokenType.PLUS, "+", null, 1, 22),
            new Token(TokenType.NUMBER, "1", 1, 1, 23),
            new Token(TokenType.SEMICOLON, ";", null, 1, 24),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 25),
            new Token(TokenType.EOF, "", null, 1, 26),
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("{ (var i = 0) (while (< i 10) { { (; (assign a (+ a 1)))} (; (assign i (+ i 1)))})}", result);
    }
    
    [Fact]
    public void ParseContinue()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.CONTINUE, "continue", null, 1, 1),
            new Token(TokenType.SEMICOLON, ";", null, 1, 2),
            new Token(TokenType.EOF, "", null, 1, 3)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("continue", result);
    }
    
    [Fact]
    public void ParseBreak()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.BREAK, "break", null, 1, 1),
            new Token(TokenType.SEMICOLON, ";", null, 1, 2),
            new Token(TokenType.EOF, "", null, 1, 3)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("break", result);
    }
    
    // Functions
    [Fact]
    public void ParseFunctionDeclaration_WithoutParameters()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FUN, "fun", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 2),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 4),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 5),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 6),
            new Token(TokenType.EOF, "", null, 1, 7)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(fun foo() )", result);
    }

    [Fact]
    public void ParseFunctionDeclaration_WithParameters()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FUN, "fun", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 2),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 4),
            new Token(TokenType.COMMA, ",", null, 1, 5),
            new Token(TokenType.IDENTIFIER, "b", "b", 1, 6),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 7),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 8),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 9),
            new Token(TokenType.EOF, "", null, 1, 10)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(fun foo(a b) )", result);
    }

    [Fact]
    public void ParseFunctionDeclaration_WithBody()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FUN, "fun", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 2),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 4),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 5),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 6),
            new Token(TokenType.EQUAL, "=", null, 1, 7),
            new Token(TokenType.NUMBER, "1", 1, 1, 8),
            new Token(TokenType.SEMICOLON, ";", null, 1, 9),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 10),
            new Token(TokenType.EOF, "", null, 1, 11)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(fun foo() (; (assign a 1)))", result);
    }
    
    [Fact]
    public void ParseFunctionDeclaration_FailsWithTooManyArguments()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FUN, "fun", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 2),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
        };

        for (int i = 0; i < 511; i += 2)
        {
            tokens.Add(new Token(TokenType.IDENTIFIER, $"arg{i}", $"arg{i}", 1, i + 4));
            if (i < 510)
            {
                tokens.Add(new Token(TokenType.COMMA, ",", null, 1, i + 5));
            }
        }
        
        tokens.Add(new Token(TokenType.RIGHT_PAREN, ")", null, 1, 610));
        tokens.Add(new Token(TokenType.LEFT_BRACE, "{", null, 1, 611));
        tokens.Add(new Token(TokenType.RIGHT_BRACE, "}", null, 1, 612));
        tokens.Add(new Token(TokenType.EOF, "", null, 1, 613));

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(message => message.Contains("PS001"))), Times.Once);
    }
    
    
    [Fact]
    public void ParseFunction_WithNoParameterNameAfterComma_ReportsError()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FUN, "fun", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 2),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
            new Token(TokenType.COMMA, ",", null, 1, 4),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 5),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 6),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 7),
            new Token(TokenType.EOF, "", null, 1, 8)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(x => x.Contains("PS003"))), Times.Once());
    }
    
    [Fact]
    public void ParseFunction_WithMissingRightParenthesis_ReportsError()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FUN, "fun", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 2),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 5),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 6),
            new Token(TokenType.EOF, "", null, 1, 7)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(x => x.Contains("PS003"))), Times.Once());
    }   
    
    [Fact]
    public void ParseFunction_WithMissingLeftBrace_ReportsError()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.FUN, "fun", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 2),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 4),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 6),
            new Token(TokenType.EOF, "", null, 1, 7)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(x => x.Contains("PS005"))), Times.Once());
    }
    
    [Fact]
    public void ParseFunctionCall_WithoutArguments()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 3),
            new Token(TokenType.SEMICOLON, ";", null, 1, 4),
            new Token(TokenType.EOF, "", null, 1, 5)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(; (call foo))", result);
    }
    
    [Fact]
    public void ParseFunctionCall_WithArguments()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
            new Token(TokenType.NUMBER, "1", 1, 1, 3),
            new Token(TokenType.COMMA, ",", null, 1, 4),
            new Token(TokenType.NUMBER, "2", 2, 1, 5),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 6),
            new Token(TokenType.SEMICOLON, ";", null, 1, 7),
            new Token(TokenType.EOF, "", null, 1, 8)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(; (call foo 1 2))", result);
    }

    [Fact]
    public void ParseFunctionCall_FailsWithTooManyArguments()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 1),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 2),
        };
        
        for (int i = 0; i < 511; i += 2)
        {
            tokens.Add(new Token(TokenType.NUMBER, $"{i}", i, 1, i + 3));
            if (i < 510)
            {
                tokens.Add(new Token(TokenType.COMMA, ",", null, 1, i + 4));
            }
        }
        
        tokens.Add(new Token(TokenType.RIGHT_PAREN, ")", null, 1, 512));
        tokens.Add(new Token(TokenType.SEMICOLON, ";", null, 1, 513));
        tokens.Add(new Token(TokenType.EOF, "", null, 1, 514));
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(message => message.Contains("PS002"))), Times.Once);
    }
    
    // Classes
    [Fact]
    public void ParseClassDeclaration_WithoutMethods()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.CLASS, "class", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "Foo", "Foo", 1, 2),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 3),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 4),
            new Token(TokenType.EOF, "", null, 1, 5)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(class Foo )", result);
    }

    [Fact]
    public void ParseClassDeclaration_WithMethods()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.CLASS, "class", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "Foo", "Foo", 1, 2),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 3),
            new Token(TokenType.IDENTIFIER, "bar", "bar", 1, 5),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 6),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 7),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 8),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 9),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 10),
            new Token(TokenType.EOF, "", null, 1, 11)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(class Foo (fun bar() ))", result);
    }
    
    [Fact]
    public void ParseClassDeclaration_WithInheritance()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.CLASS, "class", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "Foo", "Foo", 1, 2),
            new Token(TokenType.LESS, "<", null, 1, 3),
            new Token(TokenType.IDENTIFIER, "Bar", "Bar", 1, 4),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 5),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 6),
            new Token(TokenType.EOF, "", null, 1, 7)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(class Foo < Bar )", result);
    }
    
    [Fact]
    public void ParseClassDeclaration_WithMethodsAndInheritance()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.CLASS, "class", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "Foo", "Foo", 1, 2),
            new Token(TokenType.LESS, "<", null, 1, 3),
            new Token(TokenType.IDENTIFIER, "Bar", "Bar", 1, 4),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 5),
            new Token(TokenType.IDENTIFIER, "baz", "baz", 1, 6),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 7),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 8),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 9),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 10),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 11),
            new Token(TokenType.EOF, "", null, 1, 12)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(class Foo < Bar (fun baz() ))", result);
    }

    [Fact] public void ParseThis_InMethod()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.THIS, "this", null, 1, 1),
            new Token(TokenType.DOT, ".", null, 1, 2),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 3),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 4),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 5),
            new Token(TokenType.SEMICOLON, ";", null, 1, 6),
            new Token(TokenType.EOF, "", null, 1, 7)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(; (call (get this foo)))", result);
    }
    
    [Fact]
    public void ParseSuper_InMethod()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.SUPER, "super", null, 1, 1),
            new Token(TokenType.DOT, ".", null, 1, 2),
            new Token(TokenType.IDENTIFIER, "foo", "foo", 1, 3),
            new Token(TokenType.LEFT_PAREN, "(", null, 1, 4),
            new Token(TokenType.RIGHT_PAREN, ")", null, 1, 5),
            new Token(TokenType.SEMICOLON, ";", null, 1, 6),
            new Token(TokenType.EOF, "", null, 1, 7)
        };

        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(statement);

        Assert.Equal("(; (call (super super foo)))", result);
    }
    
    // Block and Scope
    [Fact]
    public void ParseBlock_EmptyBlock()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 1),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 2),
            new Token(TokenType.EOF, "", null, 1, 3)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("{}", result);
    }

    [Fact]
    public void ParseBlock_WithStatements()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 2),
            new Token(TokenType.EQUAL, "=", null, 1, 3),
            new Token(TokenType.NUMBER, "1", 1, 1, 4),
            new Token(TokenType.SEMICOLON, ";", null, 1, 5),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 6),
            new Token(TokenType.EOF, "", null, 1, 7)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("{ (; (assign a 1))}", result);
    }

    [Fact]
    public void ParseBlock_WithNestedBlocks()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 2),
            new Token(TokenType.EQUAL, "=", null, 1, 3),
            new Token(TokenType.NUMBER, "1", 1, 1, 4),
            new Token(TokenType.SEMICOLON, ";", null, 1, 5),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 6),
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 7),
            new Token(TokenType.IDENTIFIER, "b", "b", 1, 8),
            new Token(TokenType.EQUAL, "=", null, 1, 9),
            new Token(TokenType.NUMBER, "2", 2, 1, 10),
            new Token(TokenType.SEMICOLON, ";", null, 1, 11),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 12),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 13),
            new Token(TokenType.RIGHT_BRACE, "}", null, 1, 14),
            new Token(TokenType.EOF, "", null, 1, 15)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("{ (; (assign a 1)) { { (; (assign b 2))}}}", result);
    }
    
    [Fact]
    public void ParseBlock_WithMissingRightBrace_ReportsError()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.LEFT_BRACE, "{", null, 1, 1),
            new Token(TokenType.IDENTIFIER, "a", "a", 1, 2),
            new Token(TokenType.EQUAL, "=", null, 1, 3),
            new Token(TokenType.NUMBER, "1", 1, 1, 4),
            new Token(TokenType.SEMICOLON, ";", null, 1, 5),
            new Token(TokenType.EOF, "", null, 1, 6)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        _mockErrorReporter.Verify(r => r.Error(It.IsAny<Token>(), It.Is<string>(x => x.Contains("PS006"))), Times.Once());
    }

    // Return Statements
    [Fact]
    public void ParseReturn_NoValue()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.RETURN, "return", null, 1, 1),
            new Token(TokenType.SEMICOLON, ";", null, 1, 2),
            new Token(TokenType.EOF, "", null, 1, 3)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("(return)", result);
    }

    [Fact]
    public void ParseReturn_WithValue()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.RETURN, "return", null, 1, 1),
            new Token(TokenType.NUMBER, "1", 1, 1, 2),
            new Token(TokenType.SEMICOLON, ";", null, 1, 3),
            new Token(TokenType.EOF, "", null, 1, 4)
        };
        
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal("(return 1)", result);
    }

    // Error Recovery
    // [Fact]
    // public void ParseError_Synchronization()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // [Fact]
    // public void ParseError_MissingTerminator()
    // {
    //     throw new NotImplementedException();
    // }
    
}