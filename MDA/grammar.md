# Grammar

## Expressions

### **Literals**
- **Numbers**: Integer and floating-point literals, e.g., `123`, `3.14`.
- **Strings**: Double-quoted text, e.g., `"Hello, world!"`.
- **Booleans**: The constants `true` and `false`.
- **Null**: The `null` value representing the absence of a value.

### **Unary Expressions**
- Prefix operators:
    - **`!`**: Logical NOT.
    - **`-`**: Negation of a number.
- Suffix operators:
    - **`++`**: Increment a variable by 1.
    - **`--`**: Decrement a variable by 1.

### **Binary Expressions**
- Infix arithmetic operators: `+`, `-`, `*`, `/`, `%`.
- Logical operators: `==`, `!=`, `<`, `<=`, `>`, `>=`.

### **Parentheses**
- Grouping for precedence: `(expression)`.

#### Example of expression
```
1 - (2 * 3) < 4 == false
```

### **Grammar for Expressions**
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
---

## **Program Structure**

A `mda` program consists of a series of declarations and statements. It ends with an end-of-file marker (EOF).

### **Grammar for Program Structure**
```plaintext
program        → declaration* EOF ;
```
---

## **Declarations**

### **Class Declarations**
Define object-oriented structures.
```plaintext
classDecl      → "class" IDENTIFIER ( "<" IDENTIFIER )?
                 "{" function* "}" ;
```

### **Function Declarations**
Define reusable blocks of code.
```plaintext
funDecl        → "fun" function ;

function       → IDENTIFIER "(" parameters? ")" block ;

parameters     → IDENTIFIER ( "," IDENTIFIER )* ;
```

### **Variable Declarations**
Introduce new variables.
```plaintext
varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
```
---


## **Statements**

### **Expression Statements**
Evaluate an expression.
```plaintext
exprStmt       → expression ";" ;
```

### **Print Statements**
Print a value to the console.
```plaintext
printStmt      → "print" expression ";" ;
```

### **Return Statements**
Return a value from a function.
```plaintext
returnStmt     → "return" expression? ";" ;
```

### **Control Flow Statements**
- **For Loops**: Iterative loops.
  ```plaintext
  forStmt        → "for" "(" ( varDecl | exprStmt | ";" )
                   expression? ";"
                   expression? ")" statement ;
  ```
- **While Loops**: Conditional loops.
  ```plaintext
  whileStmt      → "while" "(" expression ")" statement ;
  ```
- **If Statements**: Conditional branching.
  ```plaintext
  ifStmt         → "if" "(" expression ")" statement
                 ( "else" statement )? ;
  ```
- **Break Statements**: Exit a loop.
  ```plaintext
  breakStmt      → "break" ";" ;
  ```
- **Continue Statements**: Skip to the next iteration of a loop.
  ```plaintext
  continueStmt   → "continue" ";" ;
  ```

### **Blocks**
Group multiple declarations or statements.
```plaintext
block          → "{" declaration* "}" ;
```
---

## **Expressions (Extended)**

### **Assignment**
Assign values to variables or modify them with compound operators.
```plaintext
assignment     → ( call "." )? IDENTIFIER ( "=" assignment
                                          | "+=" assignment
                                          | "-=" assignment
                                          | "*=" assignment
                                          | "/=" assignment
                                          | "%=" assignment )
               | logic_or ;
```

### **Logical Operators**
- **OR**: `expression or expression`
- **AND**: `expression and expression`
```plaintext
logic_or       → logic_and ( "or" logic_and )* ;
logic_and      → equality ( "and" equality )* ;
```

### **Equality and Comparison**
```plaintext
equality       → comparison ( ( "!=" | "==" ) comparison )* ;
comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
```

### **Arithmetic Operations**
```plaintext
term           → factor ( ( "-" | "+" ) factor )* ;
factor         → unary ( ( "/" | "*" | "%" ) unary )* ;
```

### **Unary Operators**
- **Logical NOT**: `!expression`
- **Negation**: `-expression`
- **Increment/Decrement**: `++variable` or `--variable`
```plaintext
unary          → ( "!" | "-" | "++" | "--" ) unary | call ;
```

### **Function and Method Calls**
```plaintext
call           → primary ( "(" arguments? ")" | "." IDENTIFIER )* ;
arguments      → expression ( "," expression )* ;
```

### **Primary Expressions**
Base expressions like literals and identifiers.
```plaintext
primary        → "true" | "false" | "null" | "this"
               | NUMBER | STRING | IDENTIFIER | "(" expression ")"
               | "super" "." IDENTIFIER ;
```

---

## **Complete Grammar Specification**

```
program        → declaration* EOF ;

declaration    → classDecl
               | funDecl
               | varDecl
               | statement ;

classDecl      → "class" IDENTIFIER ( "<" IDENTIFIER )?
                 "{" function* "}" ;
               
funDecl        → "fun" function ;

function       → IDENTIFIER "(" parameters? ")" block ;

parameters     → IDENTIFIER ( "," IDENTIFIER )* ;

varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;

statement      → exprStmt
               | forStmt
               | ifStmt
               | printStmt
               | returnStmt
               | whileStmt
               | breakStmt
               | continueStmt
               | block ;
               
returnStmt     -> "return" expression? ";" ;

breakStmt      → "break" ";" ;

continueStmt   → "continue" ";" ;

forStmt        → "for" "(" ( varDecl | exprStmt | ";" )
                 expression? ";"
                 expression? ")" statement ;

whileStmt      → "while" "(" expression ")" statement ;

ifStmt         → "if" "(" expression ")" statement
               ( "else" statement )? ;

block          → "{" declaration* "}" ;

exprStmt       → expression ";" ;

printStmt      → "print" expression ";" ;

expression     → assignment ;

assignment     → ( call "." )? IDENTIFIER ( "=" assignment
                                          | "+=" assignment
                                          | "-=" assignment
                                          | "*=" assignment
                                          | "/=" assignment
                                          | "%=" assignment )
               | logic_or ;
               
logic_or       → logic_and ( "or" logic_and )* ;

logic_and      → equality ( "and" equality )* ;

equality       → comparison ( ( "!=" | "==" ) comparison )* ;

comparison     → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;

term           → factor ( ( "-" | "+" ) factor )* ;

factor         → unary ( ( "/" | "*" ) unary )* ;

unary          → ( "!" | "-" | "++" | "--" ) unary | call ;

call           → primary ( "(" arguments? ")" | "." IDENTIFIER )* ;

arguments      → expression ( "," expression )* ;
               
primary        → "true" | "false" | "null" | "this"
               | NUMBER | STRING | IDENTIFIER | "(" expression ")"
               | "super" "." IDENTIFIER ;
```