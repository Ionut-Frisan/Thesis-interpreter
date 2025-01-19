namespace MDA.Tests.Unit.Parser;

public class ExpressionParsingTests
{
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseLiteralExpressionData))]
    public void ParseLiteralExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseUnaryExpressionData))]
    public void ParseUnaryExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseChainedUnaryExpressionData))]
    public void ParseChainedUnaryExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }

    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseIncrementDecrementExpressionData))]
    public void ParseIncrementDecrementExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseBinaryExpressionData))]
    public void ParseBinaryExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseChainedBinaryExpressionData))]
    public void ParseChainedBinaryExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseLogicalExpressionData))]
    public void ParseLogicalExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseChainedLogicalExpressionData))]
    public void ParseChainedLogicalExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(ExpressionParsingData.ParseGroupingExpressionData))]
    public void ParseGroupingExpression(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var expression = parser.Parse();

        var printer = new AstPrinter();
        var result = printer.Print(expression);

        Assert.Equal(expected, result);
    }
}