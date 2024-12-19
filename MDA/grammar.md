# Grammar

## Expressions

### Literals
Numbers, strings, Booleans, and null.

### Unary Expression
A prefix ! to perform a logical not, and - to negate a number.

### Binary expressions
The infix arithmetic (+, -, *, /) and logic operators (==, !=, <, <=, >, >=).

### Parentheses
A pair of ( and ) wrapped around an expression.

#### Example of expression
```
1 - (2 * 3) < 4 == false
```

## Grammar

```plaintext
expression     → literal
                | unary
                | binary
                | grouping ;

literal        → NUMBER | STRING | "true" | "false" | "null" ;

grouping       → "(" expression ")" ;

unary          → ( "-" | "!" ) expression ;

binary         → expression operator expression ;

operator       → "==" | "!=" | "<" | "<=" | ">" | ">="
                | "+"  | "-"  | "*" | "/" ;
```

## TODO: Not sure what this is https://craftinginterpreters.com/parsing-expressions.html

```
program        → declaration* EOF ;

declaration    → varDecl
               | statement ;

varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;

statement      → exprStmt
               | printStmt ;

exprStmt       → expression ";" ;

printStmt      → "print" expression ";" ;

expression     → assignment ;

assignment     → IDENTIFIER "=" assignment
               | equality ;

equality       → comparison ( ( "!=" | "==" ) comparison )* ;

comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;

term           → factor ( ( "-" | "+" ) factor )* ;

factor         → unary ( ( "/" | "*" ) unary )* ;

unary          → ( "!" | "-" ) unary
               | primary ;
               
primary        → "true" | "false" | "nil"
               | NUMBER | STRING
               | "(" expression ")"
               | IDENTIFIER ;
```