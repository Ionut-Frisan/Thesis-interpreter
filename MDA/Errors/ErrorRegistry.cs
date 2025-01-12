namespace MDA.Errors;

public class ErrorRegistry
{
    private static readonly Dictionary<string, Error> Errors = new()
    {
        {
            "SC001", new Error(
                "SC001",
                "Unrecognized character ':character:'",
                ErrorCategory.SCANNER,
                ErrorLevel.ERROR,
                "The character is not recognized by the scanner."
            )
        },
        {
            "SC002", new Error(
                "SC002",
                "Unterminated string",
                ErrorCategory.SCANNER,
                ErrorLevel.ERROR,
                "The string is not terminated by a closing quote."
            )
        },
        
        // Parser errors
        {
            "PS001", new Error(
                "PS001",
                "Can't have more than 255 parameters.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "The maximum number of parameters a function can have is 255."
            )
        },
        {
            "PS002", new Error(
                "PS002",
                "Can't have more than 255 arguments.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "The maximum number of arguments a function can have is 255."
            )
        },
        {
            "PS003", new Error(
                "PS003",
                "Expected parameter name.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A parameter name is expected."
            )
        },
        {
            "PS004", new Error(
                "PS004",
                "Expected ')' after parameters.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing parenthesis is expected after parameters list."
            )
        },
        {
            "PS005", new Error(
                "PS005",
                "Expected '{' before :kind: body.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Opening curly brace is expected before :kind: body."
            )
        },
        {
            "PS006", new Error(
                "PS006",
                "Expect '}' after block.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing curly brace is expected after block statement."
            )
        },
        {
            "PS007", new Error(
                "PS007",
                "Invalid assignment target.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "The left-hand side of an assignment must be a variable."
            )
        },
        {
            "PS008", new Error(
                "PS008",
                "Invalid assignment target for compound assignment operator.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "The left-hand side of a compound assignment must be a variable."
            )
        },
        {
            "PS009", new Error(
                "PS009",
                "Invalid assignment target for increment/decrement operator.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "The left-hand side of a increment/decrement operator must be a variable."
            )
        },
        {
            "PS010", new Error(
                "PS010",
                "Expected property name after '.'.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A property name is expected after '.'."
            )
        },
        {
            "PS011", new Error(
                "PS011",
                "Expected ')' after arguments.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing parenthesis is expected after arguments list."
            )
        },
        {
            "PS012", new Error(
                "PS012",
                "Expect '.' after 'super'.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "'super' must be followed by '.' as it is used to access superclass methods."
            )
        },
        {
            "PS013", new Error(
                "PS013",
                "Expect superclass method name.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A superclass method name is expected after 'super.'."
            )
        },
        {
            "PS014", new Error(
                "PS014",
                "Expect ')' after expression.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing parenthesis is expected after expression."
            )
        },
        {
            "PS015", new Error(
                "PS015",
                "Expect expression.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "An expression is expected."
            )
        },
        {
            "PS016", new Error(
                "PS016",
                "Expect class name.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A class name is expected after 'class' keyword."
            )
        },
        {
            "PS017", new Error(
                "PS017",
                "Expect superclass name.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A superclass name is expected after '>' in class declaration."
            )
        },
        {
            "PS018", new Error(
                "PS018",
                "Expect '{' before class body.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Opening curly brace is expected before class body."
            )
        },
        {
            "PS019", new Error(
                "PS019",
                "Expect '}' after class body.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Opening curly brace is expected after class body."
            )
        },
        {
            "PS020", new Error(
                "PS020",
                "Expected '(' after 'for'.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Opening parenthesis is expected after 'for' keyword."
            )
        },
        {
            "PS021", new Error(
                "PS021",
                "Expected ';' after 'for' condition.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Semicolon is expected after 'for' condition."
            )
        },
        {
            "PS022", new Error(
                "PS022",
                "Expected ')' after 'for' clauses.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing parenthesis is expected after 'for' clauses."
            )
        },
        {
            "PS023", new Error(
                "PS023",
                "Expected variable name.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A variable name is expected after 'var' keyowrd."
            )
        },
        {
            "PS024", new Error(
                "PS024",
                "Expected ';' after variable declaration.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Semicolon is expected after variable declaration."
            )
        },
        {
            "PS025", new Error(
                "PS025",
                "Expected '(' after 'while'.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Opening parenthesis is expected after 'while' keyword."
            )
        },
        {
            "PS026", new Error(
                "PS026",
                "Expected ')' after 'while' condition.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing parenthesis is expected after 'while' condition."
            )
        },
        {
            "PS027", new Error(
                "PS027",
                "Expected '(' after 'if'.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Opening parenthesis is expected after 'if' keyword."
            )
        },
        {
            "PS028", new Error(
                "PS028",
                "Expected ')' after 'if' condition.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing parenthesis is expected after 'if' condition."
            )
        },
        {
            "PS029", new Error(
                "PS029",
                "Expect ';' after value.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Semicolon is expected after value."
            )
        },
        {
            "PS030", new Error(
                "PS030",
                "Expected ';' after return value.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Semicolon is expected after return value."
            )
        },
        {
            "PS031", new Error(
                "PS031",
                "Expect ';' after 'continue'.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Semicolon is expected after 'continue' keyword."
            )
        },
        {
            "PS032", new Error(
                "PS032",
                "Expect ';' after 'break'.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Semicolon is expected after 'break' keyword."
            )
        },
        {
            "PS033", new Error(
                "PS033",
                "Expect ';' after expression.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Semicolon is expected after expression."
            )
        },
        {
            "PS034", new Error(
                "PS034",
                "Expect :kind: name.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Expected an identifier when declaring a :kind:."
            )
        },
        {
            "PS035", new Error(
                "PS035",
                "Expected '(' after :kind: name.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Opening parenthesis is expected after :kind: name."
            )
        },
        {
            "PS036", new Error(
                "PS036",
                "Cannot have more than 255 elements in a list.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "The maximum number of elements a list can have is 255."
            )
        },
        {
            "PS037", new Error(
                "PS037",
                "Expected ']' after list elements.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing bracket is expected after list elements."
            )
        },
        {
            "PS038", new Error(
                "PS038",
                "Expected ']' after list index.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Closing bracket is expected after list index."
            )
        },
        
        // Resolver errors
        {
            "RS001", new Error(
                "RS001",
                "A class cannot inherit from itself.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A class cannot inherit from itself."
            )
        },
        {
            "RS002", new Error(
                "RS002",
                "Cannot return from top-level code.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Return statement is not allowed in top-level code."
            )
        },
        {
            "RS003", new Error(
                "RS003",
                "Cannot return a value from an initializer.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Return statement with a value is not allowed in an initializer."
            )
        },
        {
            "RS004", new Error(
                "RS004",
                "Cannot use 'break' outside of a loop.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Break statement is not allowed outside of a loop."
            )
        },
        {
            "RS005", new Error(
                "RS005",
                "Cannot use 'continue' outside of a loop.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Continue statement is not allowed outside of a loop."
            )
        },
        {
            "RS006", new Error(
                "RS006",
                "Cannot use 'super' outside of a class.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Super keyword is not allowed outside of a class."
            )
        },
        {
            "RS007", new Error(
                "RS007",
                "Cannot use 'super' in a class with no superclass.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "Super keyword is not allowed in a class with no superclass."
            )
        },
        {
            "RS008", new Error(
                "RS008",
                "Cannot use 'this' outside of a class.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "This keyword is not allowed outside of a class."
            )
        },
        {
            "RS009", new Error(
                "RS009",
                "Cannot read local variable in its own initializer.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A local variable cannot be read in its own initializer."
            )
        },
        {
            "RS010", new Error(
                "RS010",
                "Variable ':name:' is already declared in this scope.",
                ErrorCategory.PARSER,
                ErrorLevel.ERROR,
                "A variable with the same name is already declared in this scope."
            )
        },
    };
    
    
    public static Error? GetError(string id)
    {
        if (!Errors.ContainsKey(id))
        {
            return null;
        }
        return Errors[id];
    }
    
}