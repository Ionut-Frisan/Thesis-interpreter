using System.Collections;

namespace MDA.Tests.Unit.Parser;

public class StatementParsingData
{
    public class ParseCompoundAssignmentData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var operators = new Dictionary<string, TokenType>
            {
                { "+=", TokenType.PLUS_EQUAL },
                { "-=", TokenType.MINUS_EQUAL },
                { "*=", TokenType.STAR_EQUAL },
                { "/=", TokenType.SLASH_EQUAL },
                { "%=", TokenType.PERCENT_EQUAL }
            };

            foreach (var op in operators)
            {
                yield return new object[]
                {
                    new List<Token>
                    {
                        new Token(TokenType.IDENTIFIER, "a", "a", 1, 1),
                        new Token(op.Value, op.Key, op.Key, 1, 2),
                        new Token(TokenType.NUMBER, "1", 1, 1, 3),
                        new Token(TokenType.SEMICOLON, ";", null, 1, 4),
                        new Token(TokenType.EOF, "", null, 1, 5)
                    },
                    $"(; (assign a ({op.Key[0]} a 1)))"
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}