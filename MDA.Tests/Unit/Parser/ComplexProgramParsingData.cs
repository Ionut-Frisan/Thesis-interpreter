using System.Collections;

namespace MDA.Tests.Unit.Parser;

public class ComplexProgramParsingData
{
    public class ParseComplexProgramData : IEnumerable<object[]>
    {
        // TODO: Add more complex programs to test
        public IEnumerator<object[]> GetEnumerator()
        {
            /*
                var a = 1;
                var b = 2;
                var result = a + b;
             */
            yield return new object[]
            {
                new List<Token>
                {
                    new Token(TokenType.VAR, "var", null, 1, 1),
                    new Token(TokenType.IDENTIFIER, "a", "a", 1, 2),
                    new Token(TokenType.EQUAL, "=", null, 1, 3),
                    new Token(TokenType.NUMBER, "1", 1, 1, 4),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 5),
                    new Token(TokenType.VAR, "var", null, 1, 6),
                    new Token(TokenType.IDENTIFIER, "b", "b", 1, 7),
                    new Token(TokenType.EQUAL, "=", null, 1, 8),
                    new Token(TokenType.NUMBER, "2", 2, 2, 9),
                    new Token(TokenType.SEMICOLON, ";", null, 2, 10),
                    new Token(TokenType.VAR, "var", null, 2, 11),
                    new Token(TokenType.IDENTIFIER, "result", "result", 2, 12),
                    new Token(TokenType.EQUAL, "=", null, 2, 13),
                    new Token(TokenType.IDENTIFIER, "a", "a", 2, 14),
                    new Token(TokenType.PLUS, "+", null, 2, 15),
                    new Token(TokenType.IDENTIFIER, "b", "b", 2, 16),
                    new Token(TokenType.SEMICOLON, ";", null, 2, 17),
                    new Token(TokenType.EOF, "", null, 2, 18)
                },
                "(var a = 1)(var b = 2)(var result = (+ a b))"
            };

            /*
                var a = 1;
                var b = 2;
                var result = a + b;
                var average = (a + b) / 2;
             */
            yield return new object[]
            {
                new List<Token>
                {
                    new Token(TokenType.VAR, "var", null, 1, 1),
                    new Token(TokenType.IDENTIFIER, "a", "a", 1, 2),
                    new Token(TokenType.EQUAL, "=", null, 1, 3),
                    new Token(TokenType.NUMBER, "1", 1, 1, 4),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 5),
                    new Token(TokenType.VAR, "var", null, 1, 6),
                    new Token(TokenType.IDENTIFIER, "b", "b", 1, 7),
                    new Token(TokenType.EQUAL, "=", null, 1, 8),
                    new Token(TokenType.NUMBER, "2", 2, 2, 9),
                    new Token(TokenType.SEMICOLON, ";", null, 2, 10),
                    new Token(TokenType.VAR, "var", null, 2, 11),
                    new Token(TokenType.IDENTIFIER, "result", "result", 2, 12),
                    new Token(TokenType.EQUAL, "=", null, 2, 13),
                    new Token(TokenType.IDENTIFIER, "a", "a", 2, 14),
                    new Token(TokenType.PLUS, "+", null, 2, 15),
                    new Token(TokenType.IDENTIFIER, "b", "b", 2, 16),
                    new Token(TokenType.SEMICOLON, ";", null, 2, 17),
                    new Token(TokenType.VAR, "var", null, 2, 18),
                    new Token(TokenType.IDENTIFIER, "average", "average", 2, 19),
                    new Token(TokenType.EQUAL, "=", null, 2, 20),
                    new Token(TokenType.LEFT_PAREN, "(", null, 2, 21),
                    new Token(TokenType.IDENTIFIER, "a", "a", 2, 22),
                    new Token(TokenType.PLUS, "+", null, 2, 23),
                    new Token(TokenType.IDENTIFIER, "b", "b", 2, 24),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 2, 25),
                    new Token(TokenType.SLASH, "/", null, 2, 26),
                    new Token(TokenType.NUMBER, "2", 2, 2, 27),
                    new Token(TokenType.SEMICOLON, ";", null, 2, 28),
                    new Token(TokenType.EOF, "", null, 2, 29)
                },
                "(var a = 1)(var b = 2)(var result = (+ a b))(var average = (/ (group (+ a b)) 2))"
            };

            /*
                var i = 0;
                while (i < 10) {
                    if (i == 3) {
                        continue;
                    }
                    print i;
                    i = i + 1;
                    if (i == 5) {
                        break;
                    }
                }
             */
            yield return new object[]
            {
                new List<Token>
                {
                    new Token(TokenType.VAR, "var", null, 1, 1),
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 2),
                    new Token(TokenType.EQUAL, "=", null, 1, 3),
                    new Token(TokenType.NUMBER, "0", 0, 1, 4),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 5),
                    new Token(TokenType.WHILE, "while", null, 1, 6),
                    new Token(TokenType.LEFT_PAREN, "(", null, 1, 7),
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 8),
                    new Token(TokenType.LESS, "<", null, 1, 9),
                    new Token(TokenType.NUMBER, "10", 10, 1, 10),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 1, 11),
                    new Token(TokenType.LEFT_BRACE, "{", null, 1, 12),
                    new Token(TokenType.IF, "if", null, 1, 13),
                    new Token(TokenType.LEFT_PAREN, "(", null, 1, 14),
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 15),
                    new Token(TokenType.EQUAL_EQUAL, "==", null, 1, 16),
                    new Token(TokenType.NUMBER, "3", 3, 1, 17),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 1, 18),
                    new Token(TokenType.LEFT_BRACE, "{", null, 1, 19),
                    new Token(TokenType.CONTINUE, "continue", null, 1, 20),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 21),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 1, 22),
                    new Token(TokenType.PRINT, "print", null, 1, 23),
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 24),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 25),
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 26),
                    new Token(TokenType.EQUAL, "=", null, 1, 27),
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 28),
                    new Token(TokenType.PLUS, "+", null, 1, 29),
                    new Token(TokenType.NUMBER, "1", 1, 1, 30),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 31),
                    new Token(TokenType.IF, "if", null, 1, 32),
                    new Token(TokenType.LEFT_PAREN, "(", null, 1, 33),
                    new Token(TokenType.IDENTIFIER, "i", "i", 1, 34),
                    new Token(TokenType.EQUAL_EQUAL, "==", null, 1, 35),
                    new Token(TokenType.NUMBER, "5", 5, 1, 36),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 1, 37),
                    new Token(TokenType.LEFT_BRACE, "{", null, 1, 38),
                    new Token(TokenType.BREAK, "break", null, 1, 39),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 40),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 1, 41),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 1, 42),
                    new Token(TokenType.EOF, "", null, 1, 43)
                },
                "(var i = 0)(while (< i 10) { (if (== i 3) { continue}) (print i) (; (assign i (+ i 1))) (if (== i 5) { break})})"
            };

            /*
             class Person {
                 init(name, age) {
                   this.name = name;
                   this.age = age;
                 }
                 greet() {
                   return "Hello, my name is " + this.name + " and I am " + this.age + " years old.";
                 }
               }
               
               class Employee < Person {
                 init(name, age, job) {
                   super.init(name, age);
                   this.job = job;
                 }
                 greet() {
                   return super.greet() + " I work as a " + this.job + ".";
                 }
               }
               
               var person = Person("John", 30);
               var employee = Employee("Jane", 25, "Developer");
               
               print person.greet();
               print employee.greet();
             */
            yield return new object[]
            {
                new List<Token>
                {
                    new Token(TokenType.CLASS, "class", null, 1, 1),
                    new Token(TokenType.IDENTIFIER, "Person", "Person", 1, 2),
                    new Token(TokenType.LEFT_BRACE, "{", null, 1, 3),
                    new Token(TokenType.IDENTIFIER, "init", "init", 2, 4),
                    new Token(TokenType.LEFT_PAREN, "(", null, 2, 5),
                    new Token(TokenType.IDENTIFIER, "name", "name", 2, 6),
                    new Token(TokenType.COMMA, ",", null, 2, 7),
                    new Token(TokenType.IDENTIFIER, "age", "age", 2, 8),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 2, 9),
                    new Token(TokenType.LEFT_BRACE, "{", null, 2, 10),
                    new Token(TokenType.THIS, "this", null, 3, 11),
                    new Token(TokenType.DOT, ".", null, 3, 12),
                    new Token(TokenType.IDENTIFIER, "name", "name", 3, 13),
                    new Token(TokenType.EQUAL, "=", null, 3, 14),
                    new Token(TokenType.IDENTIFIER, "name", "name", 3, 15),
                    new Token(TokenType.SEMICOLON, ";", null, 3, 16),
                    new Token(TokenType.THIS, "this", null, 4, 17),
                    new Token(TokenType.DOT, ".", null, 4, 18),
                    new Token(TokenType.IDENTIFIER, "age", "age", 4, 19),
                    new Token(TokenType.EQUAL, "=", null, 4, 20),
                    new Token(TokenType.IDENTIFIER, "age", "age", 4, 21),
                    new Token(TokenType.SEMICOLON, ";", null, 4, 22),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 5, 23),
                    new Token(TokenType.IDENTIFIER, "greet", "greet", 6, 24),
                    new Token(TokenType.LEFT_PAREN, "(", null, 6, 25),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 6, 26),
                    new Token(TokenType.LEFT_BRACE, "{", null, 6, 27),
                    new Token(TokenType.RETURN, "return", null, 7, 28),
                    new Token(TokenType.STRING, "\"Hello, my name is \"", "Hello, my name is ", 7, 29),
                    new Token(TokenType.PLUS, "+", null, 7, 30),
                    new Token(TokenType.THIS, "this", null, 7, 31),
                    new Token(TokenType.DOT, ".", null, 7, 32),
                    new Token(TokenType.IDENTIFIER, "name", "name", 7, 33),
                    new Token(TokenType.PLUS, "+", null, 7, 34),
                    new Token(TokenType.STRING, "\" and I am \"", " and I am ", 7, 35),
                    new Token(TokenType.PLUS, "+", null, 7, 36),
                    new Token(TokenType.THIS, "this", null, 7, 37),
                    new Token(TokenType.DOT, ".", null, 7, 38),
                    new Token(TokenType.IDENTIFIER, "age", "age", 7, 39),
                    new Token(TokenType.PLUS, "+", null, 7, 40),
                    new Token(TokenType.STRING, "\" years old.\"", " years old.", 7, 41),
                    new Token(TokenType.SEMICOLON, ";", null, 7, 42),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 8, 43),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 9, 44),
                    new Token(TokenType.CLASS, "class", null, 10, 45),
                    new Token(TokenType.IDENTIFIER, "Employee", "Employee", 10, 46),
                    new Token(TokenType.LESS, "<", null, 10, 47),
                    new Token(TokenType.IDENTIFIER, "Person", "Person", 10, 48),
                    new Token(TokenType.LEFT_BRACE, "{", null, 10, 49),
                    new Token(TokenType.IDENTIFIER, "init", "init", 11, 50),
                    new Token(TokenType.LEFT_PAREN, "(", null, 11, 51),
                    new Token(TokenType.IDENTIFIER, "name", "name", 11, 52),
                    new Token(TokenType.COMMA, ",", null, 11, 53),
                    new Token(TokenType.IDENTIFIER, "age", "age", 11, 54),
                    new Token(TokenType.COMMA, ",", null, 11, 55),
                    new Token(TokenType.IDENTIFIER, "job", "job", 11, 56),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 11, 57),
                    new Token(TokenType.LEFT_BRACE, "{", null, 11, 58),
                    new Token(TokenType.SUPER, "super", null, 12, 59),
                    new Token(TokenType.DOT, ".", null, 12, 60),
                    new Token(TokenType.IDENTIFIER, "init", "init", 12, 61),
                    new Token(TokenType.LEFT_PAREN, "(", null, 12, 62),
                    new Token(TokenType.IDENTIFIER, "name", "name", 12, 63),
                    new Token(TokenType.COMMA, ",", null, 12, 64),
                    new Token(TokenType.IDENTIFIER, "age", "age", 12, 65),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 12, 66),
                    new Token(TokenType.SEMICOLON, ";", null, 12, 67),
                    new Token(TokenType.THIS, "this", null, 13, 68),
                    new Token(TokenType.DOT, ".", null, 13, 69),
                    new Token(TokenType.IDENTIFIER, "job", "job", 13, 70),
                    new Token(TokenType.EQUAL, "=", null, 13, 71),
                    new Token(TokenType.IDENTIFIER, "job", "job", 13, 72),
                    new Token(TokenType.SEMICOLON, ";", null, 13, 73),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 14, 74),
                    new Token(TokenType.IDENTIFIER, "greet", "greet", 15, 75),
                    new Token(TokenType.LEFT_PAREN, "(", null, 15, 76),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 15, 77),
                    new Token(TokenType.LEFT_BRACE, "{", null, 15, 78),
                    new Token(TokenType.RETURN, "return", null, 16, 79),
                    new Token(TokenType.SUPER, "super", null, 16, 80),
                    new Token(TokenType.DOT, ".", null, 16, 81),
                    new Token(TokenType.IDENTIFIER, "greet", "greet", 16, 82),
                    new Token(TokenType.LEFT_PAREN, "(", null, 16, 83),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 16, 84),
                    new Token(TokenType.PLUS, "+", null, 16, 85),
                    new Token(TokenType.STRING, "\" I work as a \"", " I work as a ", 16, 86),
                    new Token(TokenType.PLUS, "+", null, 16, 87),
                    new Token(TokenType.THIS, "this", null, 16, 88),
                    new Token(TokenType.DOT, ".", null, 16, 89),
                    new Token(TokenType.IDENTIFIER, "job", "job", 16, 90),
                    new Token(TokenType.PLUS, "+", null, 16, 91),
                    new Token(TokenType.STRING, "\".\"", ".", 16, 92),
                    new Token(TokenType.SEMICOLON, ";", null, 16, 93),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 17, 94),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 18, 95),
                    new Token(TokenType.VAR, "var", null, 19, 96),
                    new Token(TokenType.IDENTIFIER, "person", "person", 19, 97),
                    new Token(TokenType.EQUAL, "=", null, 19, 98),
                    new Token(TokenType.IDENTIFIER, "Person", "Person", 19, 99),
                    new Token(TokenType.LEFT_PAREN, "(", null, 19, 100),
                    new Token(TokenType.STRING, "\"John\"", "John", 19, 101),
                    new Token(TokenType.COMMA, ",", null, 19, 102),
                    new Token(TokenType.NUMBER, "30", 30, 19, 103),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 19, 104),
                    new Token(TokenType.SEMICOLON, ";", null, 19, 105),
                    new Token(TokenType.VAR, "var", null, 20, 106),
                    new Token(TokenType.IDENTIFIER, "employee", "employee", 20, 107),
                    new Token(TokenType.EQUAL, "=", null, 20, 108),
                    new Token(TokenType.IDENTIFIER, "Employee", "Employee", 20, 109),
                    new Token(TokenType.LEFT_PAREN, "(", null, 20, 110),
                    new Token(TokenType.STRING, "\"Jane\"", "Jane", 20, 111),
                    new Token(TokenType.COMMA, ",", null, 20, 112),
                    new Token(TokenType.NUMBER, "25", 25, 20, 113),
                    new Token(TokenType.COMMA, ",", null, 20, 114),
                    new Token(TokenType.STRING, "\"Developer\"", "Developer", 20, 115),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 20, 116),
                    new Token(TokenType.SEMICOLON, ";", null, 20, 117),
                    new Token(TokenType.PRINT, "print", null, 21, 118),
                    new Token(TokenType.IDENTIFIER, "person", "person", 21, 119),
                    new Token(TokenType.DOT, ".", null, 21, 120),
                    new Token(TokenType.IDENTIFIER, "greet", "greet", 21, 121),
                    new Token(TokenType.LEFT_PAREN, "(", null, 21, 122),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 21, 123),
                    new Token(TokenType.SEMICOLON, ";", null, 21, 124),
                    new Token(TokenType.PRINT, "print", null, 22, 125),
                    new Token(TokenType.IDENTIFIER, "employee", "employee", 22, 126),
                    new Token(TokenType.DOT, ".", null, 22, 127),
                    new Token(TokenType.IDENTIFIER, "greet", "greet", 22, 128),
                    new Token(TokenType.LEFT_PAREN, "(", null, 22, 129),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 22, 130),
                    new Token(TokenType.SEMICOLON, ";", null, 22, 131),
                    new Token(TokenType.EOF, "", null, 23, 132)
                },
                "(class Person (fun init(name age) (; (set this name name))(; (set this age age)))(fun greet() (return (+ (+ (+ (+ Hello, my name is  (get this name))  and I am ) (get this age))  years old.))))(class Employee < Person (fun init(name age job) (; (call (super super init) name age))(; (set this job job)))(fun greet() (return (+ (+ (+ (call (super super greet))  I work as a ) (get this job)) .))))(var person = (call Person John 30))(var employee = (call Employee Jane 25 Developer))(print (call (get person greet)))(print (call (get employee greet)))"
            };

            /*
               fun add(a, b) { return a + b; }
               fun sub(a, b) { return a - b; }
               
               add(1, 2);
               var x = 3;
               var y = 4;
               sub(x, y);
             */
            yield return new object[]
            {
                new List<Token>
                {
                    new Token(TokenType.FUN, "fun", null, 1, 1),
                    new Token(TokenType.IDENTIFIER, "add", "add", 1, 2),
                    new Token(TokenType.LEFT_PAREN, "(", null, 1, 3),
                    new Token(TokenType.IDENTIFIER, "a", "a", 1, 4),
                    new Token(TokenType.COMMA, ",", null, 1, 5),
                    new Token(TokenType.IDENTIFIER, "b", "b", 1, 6),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 1, 7),
                    new Token(TokenType.LEFT_BRACE, "{", null, 1, 8),
                    new Token(TokenType.RETURN, "return", null, 1, 9),
                    new Token(TokenType.IDENTIFIER, "a", "a", 1, 10),
                    new Token(TokenType.PLUS, "+", null, 1, 11),
                    new Token(TokenType.IDENTIFIER, "b", "b", 1, 12),
                    new Token(TokenType.SEMICOLON, ";", null, 1, 13),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 1, 14),
                    new Token(TokenType.FUN, "fun", null, 2, 1),
                    new Token(TokenType.IDENTIFIER, "sub", "sub", 2, 2),
                    new Token(TokenType.LEFT_PAREN, "(", null, 2, 3),
                    new Token(TokenType.IDENTIFIER, "a", "a", 2, 4),
                    new Token(TokenType.COMMA, ",", null, 2, 5),
                    new Token(TokenType.IDENTIFIER, "b", "b", 2, 6),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 2, 7),
                    new Token(TokenType.LEFT_BRACE, "{", null, 2, 8),
                    new Token(TokenType.RETURN, "return", null, 2, 9),
                    new Token(TokenType.IDENTIFIER, "a", "a", 2, 10),
                    new Token(TokenType.MINUS, "-", null, 2, 11),
                    new Token(TokenType.IDENTIFIER, "b", "b", 2, 12),
                    new Token(TokenType.SEMICOLON, ";", null, 2, 13),
                    new Token(TokenType.RIGHT_BRACE, "}", null, 2, 14),
                    new Token(TokenType.IDENTIFIER, "add", "add", 3, 1),
                    new Token(TokenType.LEFT_PAREN, "(", null, 3, 2),
                    new Token(TokenType.NUMBER, "1", 1, 3, 3),
                    new Token(TokenType.COMMA, ",", null, 3, 4),
                    new Token(TokenType.NUMBER, "2", 2, 3, 5),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 3, 6),
                    new Token(TokenType.SEMICOLON, ";", null, 3, 7),
                    new Token(TokenType.VAR, "var", null, 4, 1),
                    new Token(TokenType.IDENTIFIER, "x", "x", 4, 2),
                    new Token(TokenType.EQUAL, "=", null, 4, 3),
                    new Token(TokenType.NUMBER, "3", 3, 4, 4),
                    new Token(TokenType.SEMICOLON, ";", null, 4, 5),
                    new Token(TokenType.VAR, "var", null, 5, 1),
                    new Token(TokenType.IDENTIFIER, "y", "y", 5, 2),
                    new Token(TokenType.EQUAL, "=", null, 5, 3),
                    new Token(TokenType.NUMBER, "4", 4, 5, 4),
                    new Token(TokenType.SEMICOLON, ";", null, 5, 5),
                    new Token(TokenType.IDENTIFIER, "sub", "sub", 6, 1),
                    new Token(TokenType.LEFT_PAREN, "(", null, 6, 2),
                    new Token(TokenType.IDENTIFIER, "x", "x", 6, 3),
                    new Token(TokenType.COMMA, ",", null, 6, 4),
                    new Token(TokenType.IDENTIFIER, "y", "y", 6, 5),
                    new Token(TokenType.RIGHT_PAREN, ")", null, 6, 6),
                    new Token(TokenType.SEMICOLON, ";", null, 6, 7),
                    new Token(TokenType.EOF, "", null, 7, 1)
                },
                "(fun add(a b) (return (+ a b)))(fun sub(a b) (return (- a b)))(; (call add 1 2))(var x = 3)(var y = 4)(; (call sub x y))"
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}