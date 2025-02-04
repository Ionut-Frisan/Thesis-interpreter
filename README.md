# MDA Programming Language

MDA is a programming language implemented with a recursive tree-walking interpreter. The interpreter processes the abstract syntax tree (AST) directly, making it suitable for educational purposes in language implementation.

## Core Features

### Data Types and Variables
- Numbers (integers and floating-point)
- Strings (double-quoted text)
- Booleans (`true` and `false`)
- Null value (`null`)
- Lists (dynamic arrays)
- Dynamic variable declarations with `var`
- Compound assignment operators (`+=`, `-=`, `*=`, `/=`, `%=`)

### Control Flow
- If-else statements
- While and for loops
- Break and continue statements
- Logical operators (`and`, `or`, `!`)
- Comparison operators (`==`, `!=`, `<`, `<=`, `>`, `>=`)
- Block scoping with `{}`

### Exception Handling
- Try-catch blocks for error handling
- Finally blocks for cleanup code
- Custom error classes through inheritance
- Throw statements for error propagation
- Type checking with `is` operator
- Built-in `Error` base class

### Functions
- First-class functions
- Closures
- Return statements
- Function parameters and arguments
- Nested function definitions
- Lexical scoping

### Object-Oriented Programming
- Class declarations
- Single inheritance (using `<` operator)
- Constructor methods (`init`)
- Instance methods
- `this` keyword for instance reference
- `super` keyword for parent class access
- Dynamic method dispatch

### Expressions
- Arithmetic operations (`+`, `-`, `*`, `/`, `%`)
- Unary operations (`-`, `!`)
- Increment/Decrement operators (`++`, `--`)
- Operator precedence and grouping
- String concatenation with `+`

## Code Examples

### Exception Handling
```mda
class MyCustomError < Error {}

fun throwsError() {
    throw MyCustomError("Nice error here");
}

try {
    throwsError();
} catch (e) {
    if (is(e, MyCustomError)) {
        print "MyCustomError was caught";
    }
} finally {
    print "executed in finally";
}
```

### Lists and List Operations
```mda
var list = [11, 9, 14, 1000, 9999, -11];
list.push(1);

print list.length();
list.reverse();
print list.pop();
print list.sort();
print list.contains(1000);
```

### Object-Oriented Programming
```mda
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
    init(name, age, position) {
        super.init(name, age);
        this.position = position;
    }

    promote() {
        this.position = "Senior " + this.position;
    }
}
```

### Functions and Closures
```mda
fun makeCounter() {
    var count = 0;
    fun increment() {
        count = count + 1;
        return count;
    }
    return increment;
}

var counter = makeCounter();
print counter(); // 1
print counter(); // 2
```

### Recursive Functions
```mda
fun fibonacci(n) {
    if (n <= 1) return n;
    return fibonacci(n - 1) + fibonacci(n - 2);
}

fun factorial(n) {
    if (n <= 1) return 1;
    return n * factorial(n - 1);
}
```

### Control Flow and Loops
```mda
fun findMax(n) {
    var max = 0;
    for (var i = 0; i < n; i++) {
        if (i > max) {
            max = i;
        }
    }
    return max;
}

fun countDown(start) {
    while (start > 0) {
        print start;
        start--;
    }
}
```

### Basic Data Structures

#### Stack Implementation
```mda
class Node {
    init(value) {
        this.value = value;
        this.next = null;
    }
}

class Stack {
    init() {
        this.head = null;
        this.length = 0;
    }
    
    isEmpty() {
        return this.length == 0;
    }
    
    push(value) {
        this.length++;
        var newNode = Node(value);
        newNode.next = this.head;
        this.head = newNode;
    }
    
    pop() {
        if (this.isEmpty()) {
            print "Cannot pop from an empty stack";
            return null;
        }
        this.length--;
        var tmp = this.head;
        this.head = this.head.next;
        return tmp.value;
    }
    
    peek() {
        if (this.isEmpty()) {
            print "Stack is empty";
            return null;
        }
        return this.head.value;
    }
}
```
