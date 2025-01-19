using Xunit;
using Xunit.Abstractions;

namespace MDA.Tests.Unit.Scanner;

public class TokenizationTests
{
    [Fact]
    public void EmptyInput_ReturnsEOFToken()
    {
        var scanner = new MDA.Scanner("");
        var tokens = scanner.ScanTokens();
        
        Assert.Single(tokens);
        Assert.Equal(TokenType.EOF, tokens[0].Type);
    }
    
    [Theory]
    [InlineData("(", TokenType.LEFT_PAREN)]
    [InlineData(")", TokenType.RIGHT_PAREN)]
    [InlineData("{", TokenType.LEFT_BRACE)]
    [InlineData("}", TokenType.RIGHT_BRACE)]
    [InlineData("[", TokenType.LEFT_BRACKET)]
    [InlineData("]", TokenType.RIGHT_BRACKET)]
    [InlineData(",", TokenType.COMMA)]
    [InlineData(".", TokenType.DOT)]
    [InlineData("-", TokenType.MINUS)]
    [InlineData("+", TokenType.PLUS)]
    [InlineData(";", TokenType.SEMICOLON)]
    [InlineData("/", TokenType.SLASH)]
    [InlineData("*", TokenType.STAR)]
    [InlineData("%", TokenType.PERCENT)]
    [InlineData("!", TokenType.BANG)]
    [InlineData("=", TokenType.EQUAL)]
    [InlineData("<", TokenType.LESS)]
    [InlineData(">", TokenType.GREATER)]
    public void SingleCharacterTokens_AreScannedCorrectly(string input, TokenType expectedType)
    {
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        Assert.Equal(2, tokens.Count); // Including EOF
        Assert.Equal(expectedType, tokens[0].Type);
    }
    
    [Theory]
    [InlineData("!=", TokenType.BANG_EQUAL)]
    [InlineData("==", TokenType.EQUAL_EQUAL)]
    [InlineData(">=", TokenType.GREATER_EQUAL)]
    [InlineData("<=", TokenType.LESS_EQUAL)]
    [InlineData("++", TokenType.PLUS_PLUS)]
    [InlineData("+=", TokenType.PLUS_EQUAL)]
    [InlineData("-=", TokenType.MINUS_EQUAL)]
    [InlineData("/=", TokenType.SLASH_EQUAL)]
    [InlineData("%=", TokenType.PERCENT_EQUAL)]
    [InlineData("--", TokenType.MINUS_MINUS)]
    public void TwoCharacterTokens_AreScannedCorrectly(string input, TokenType expectedType)
    {
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        Assert.Equal(2, tokens.Count); // Including EOF
        Assert.Equal(expectedType, tokens[0].Type);
    }
    
    [Theory]
    [InlineData("and", TokenType.AND)]
    [InlineData("class", TokenType.CLASS)]
    [InlineData("else", TokenType.ELSE)]
    [InlineData("false", TokenType.FALSE)]
    [InlineData("fun", TokenType.FUN)]
    [InlineData("for", TokenType.FOR)]
    [InlineData("if", TokenType.IF)]
    [InlineData("null", TokenType.NULL)]
    [InlineData("or", TokenType.OR)]
    [InlineData("print", TokenType.PRINT)]
    [InlineData("return", TokenType.RETURN)]
    [InlineData("super", TokenType.SUPER)]
    [InlineData("this", TokenType.THIS)]
    [InlineData("true", TokenType.TRUE)]
    [InlineData("var", TokenType.VAR)]
    [InlineData("while", TokenType.WHILE)]
    [InlineData("continue", TokenType.CONTINUE)]
    [InlineData("break", TokenType.BREAK)]
    public void Keywords_AreScannedCorrectly(string input, TokenType expectedType)
    {
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        Assert.Equal(2, tokens.Count); // Including EOF
        Assert.Equal(expectedType, tokens[0].Type);
    }
    
    [Theory]
    [InlineData("identifier", TokenType.IDENTIFIER)]
    [InlineData("snake_case_identifier", TokenType.IDENTIFIER)]
    [InlineData("camelCaseIdentifier", TokenType.IDENTIFIER)]
    [InlineData("\"\"", TokenType.STRING)]
    [InlineData("\"string\"", TokenType.STRING)]
    [InlineData("\"string with spaces\"", TokenType.STRING)]
    [InlineData("123", TokenType.NUMBER)]
    [InlineData("123.23", TokenType.NUMBER)]
    public void Literals_AreScannedCorrectly(string input, TokenType expectedType)
    {
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        Assert.Equal(2, tokens.Count); // Including EOF
        Assert.Equal(expectedType, tokens[0].Type);
    }
    
    [Theory]
    [InlineData("\"\"", "")]
    [InlineData("\"i am a string\"", "i am a string")]
    [InlineData("\"123\"", "123")]
    [InlineData("\"123.123\"", "123.123")]
    [InlineData("\"true\"", "true")]
    [InlineData("\"false\"", "false")]
    [InlineData("\"null\"", "null")]
    [InlineData("\"class\"", "class")]
    [InlineData("\"super\"", "super")]
    [InlineData("\"for\"", "for")]
    public void StringLiterals_AreScannedCorrectly(string input, string expectedValue)
    {
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        Assert.Equal(2, tokens.Count); // Including EOF
        Assert.Equal(TokenType.STRING, tokens[0].Type);
        Assert.Equal(expectedValue, tokens[0].Literal);
    }
    
    [Theory]
    [InlineData("0", 0)]
    [InlineData("123", 123)]
    // TODO: when running the scanner in the interpreter these values are scanned correctly but the test fails.
    [InlineData("22.0", 22)]
    [InlineData("22.5", 22.5)]
    public void NumberLiterals_AreScannedCorrectly(string input, double expectedValue)
    {
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        Assert.Equal(2, tokens.Count); // Including EOF
        Assert.Equal(TokenType.NUMBER, tokens[0].Type);
        Assert.Equal(expectedValue, tokens[0].Literal);
    }

    // TODO: need to mock the Mda.Error method to test this
    // TODO: same as above for unterminated strings
    // [Fact]
    // public void Error_IsReportedForInvalidCharacter()
    // {
    //     var scanner = new MDA.Scanner("\\");
    //     var tokens = scanner.ScanTokens();
    //     
    //     Assert.Single(tokens); // Only EOF
    // }
    
    [Fact]
    public void Whitespace_IsIgnored()
    {
        var scanner = new MDA.Scanner("  \t\n  ");
        var tokens = scanner.ScanTokens();
        
        Assert.Single(tokens); // Only EOF
    }
    
    [Fact]
    public void Comment_IsIgnored()
    {
        var input = @"
            // This is a comment
            var a = 1;
            // This is another comment
        ";
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        var expectedTypes = new[]
        {
            TokenType.VAR,
            TokenType.IDENTIFIER,
            TokenType.EQUAL,
            TokenType.NUMBER,
            TokenType.SEMICOLON,
            TokenType.EOF
        };
        
        Assert.Equal(expectedTypes.Length, tokens.Count);
        for (int i = 0; i < expectedTypes.Length; i++)
        {
            Assert.Equal(expectedTypes[i], tokens[i].Type);
        }
    }
    
    [Fact]
    public void ComplexExpression_IsScannedCorrectly()
    {
        var input = "var average = (a + b) / 2;";
        var scanner = new MDA.Scanner(input);
        var tokens = scanner.ScanTokens();
        
        var expectedTypes = new[]
        {
            TokenType.VAR,
            TokenType.IDENTIFIER,
            TokenType.EQUAL,
            TokenType.LEFT_PAREN,
            TokenType.IDENTIFIER,
            TokenType.PLUS,
            TokenType.IDENTIFIER,
            TokenType.RIGHT_PAREN,
            TokenType.SLASH,
            TokenType.NUMBER,
            TokenType.SEMICOLON,
            TokenType.EOF
        };
        
        Assert.Equal(expectedTypes.Length, tokens.Count);
        for (int i = 0; i < expectedTypes.Length; i++)
        {
            Assert.Equal(expectedTypes[i], tokens[i].Type);
        }
    }
}