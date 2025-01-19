using System.Collections;

namespace MDA.Tests.Unit.Parser;

public class ExpressionParsingData
{
    public class ParseLiteralExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var literals = new Dictionary<string, TokenType>
            {
                { "True", TokenType.TRUE },
                { "False", TokenType.FALSE },
                { "null", TokenType.NULL },
                { "1", TokenType.NUMBER },
                { "1.0", TokenType.NUMBER },
                { "\"hello\"", TokenType.STRING }
            };

            foreach (var literal in literals)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(literal.Value, literal.Key, literal.Key, 1, 1),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 2),
                        new Token(TokenType.EOF, "", null, 1, 3)
                    },
                    $"(; {literal.Key})"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseUnaryExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "-", TokenType.MINUS },
                { "!", TokenType.BANG }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(op.Value, op.Key, op.Key, 1, 1),
                        new Token(TokenType.NUMBER, "1", 1, 1, 2),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 3),
                        new Token(TokenType.EOF, "", null, 1, 4)
                    },
                    $"(; ({op.Key} 1))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseChainedUnaryExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "-", TokenType.MINUS },
                { "!", TokenType.BANG }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(op.Value, op.Key, op.Key, 1, 1),
                        new Token(op.Value, op.Key, op.Key, 1, 2),
                        new Token(TokenType.NUMBER, "1", 1, 1, 3),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 4),
                        new Token(TokenType.EOF, "", null, 1, 5)
                    },
                    $"(; ({op.Key} ({op.Key} 1)))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseIncrementDecrementExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "++", TokenType.PLUS_PLUS },
                { "--", TokenType.MINUS_MINUS }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
                        new Token(op.Value, op.Key, op.Key, 1, 2),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 3),
                        new Token(TokenType.EOF, "", null, 1, 4)
                    },
                    // a-- => a = a - 1
                    $"(; (assign a ({op.Key[0]} a 1)))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseBinaryExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "+", TokenType.PLUS },
                { "-", TokenType.MINUS },
                { "*", TokenType.STAR },
                { "/", TokenType.SLASH },
                { "%", TokenType.PERCENT },
                { "==", TokenType.EQUAL_EQUAL },
                { "!=", TokenType.BANG_EQUAL },
                { "<", TokenType.LESS },
                { ">", TokenType.GREATER },
                { "<=", TokenType.LESS_EQUAL },
                { ">=", TokenType.GREATER_EQUAL }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(TokenType.NUMBER, "1", 1, 1, 1),
                        new Token(op.Value, op.Key, op.Key, 1, 2),
                        new Token(TokenType.NUMBER, "2", 2, 1, 3),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 4),
                        new Token(TokenType.EOF, "", null, 1, 5)
                    },
                    $"(; ({op.Key} 1 2))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseChainedBinaryExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "+", TokenType.PLUS },
                { "-", TokenType.MINUS },
                { "*", TokenType.STAR },
                { "/", TokenType.SLASH },
                { "%", TokenType.PERCENT },
                { "==", TokenType.EQUAL_EQUAL },
                { "!=", TokenType.BANG_EQUAL },
                { "<", TokenType.LESS },
                { ">", TokenType.GREATER },
                { "<=", TokenType.LESS_EQUAL },
                { ">=", TokenType.GREATER_EQUAL }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(TokenType.NUMBER, "1", 1, 1, 1),
                        new Token(op.Value, op.Key, op.Key, 1, 2),
                        new Token(TokenType.NUMBER, "2", 2, 1, 3),
                        new Token(op.Value, op.Key, op.Key, 1, 4),
                        new Token(TokenType.NUMBER, "3", 3, 1, 5),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 6),
                        new Token(TokenType.EOF, "", null, 1, 7)
                    },
                    $"(; ({op.Key} ({op.Key} 1 2) 3))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseLogicalExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "and", TokenType.AND },
                { "or", TokenType.OR }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(TokenType.NUMBER, "1", 1, 1, 1),
                        new Token(op.Value, op.Key, op.Key, 1, 2),
                        new Token(TokenType.NUMBER, "2", 2, 1, 3),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 4),
                        new Token(TokenType.EOF, "", null, 1, 5)
                    },
                    $"(; ({op.Key} 1 2))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseChainedLogicalExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "and", TokenType.AND },
                { "or", TokenType.OR }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(TokenType.NUMBER, "1", 1, 1, 1),
                        new Token(op.Value, op.Key, op.Key, 1, 2),
                        new Token(TokenType.NUMBER, "2", 2, 1, 3),
                        new Token(op.Value, op.Key, op.Key, 1, 4),
                        new Token(TokenType.NUMBER, "3", 3, 1, 5),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 6),
                        new Token(TokenType.EOF, "", null, 1, 7)
                    },
                    $"(; ({op.Key} ({op.Key} 1 2) 3))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class ParseGroupingExpressionData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<Token>
                {
                    new Token(TokenType.LEFT_PAREN, "(", "(", 1, 1),
                    new Token(TokenType.NUMBER, "1", 1, 1, 2),
                    new Token(TokenType.PLUS, "+", "+", 1, 3),
                    new Token(TokenType.NUMBER, "2", 2, 1, 4),
                    new Token(TokenType.RIGHT_PAREN, ")", ")", 1, 5),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 6),
                    new Token(TokenType.EOF, "", null, 1, 7)
                },
                "(; (group (+ 1 2)))"
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}