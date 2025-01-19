namespace MDA.Tests.Unit.Parser;

public class ComplexProgramParsingTest
{
    [Theory]
    [ClassData(typeof(ComplexProgramParsingData.ParseComplexProgramData))]
    public void ParseComplexProgram(List<Token> tokens, string expected)
    {
        var parser = new MDA.Parser(tokens);
        var statement = parser.Parse();
        
        var printer = new AstPrinter();
        var result = printer.Print(statement);
        
        Assert.Equal(expected, result);
    }
}