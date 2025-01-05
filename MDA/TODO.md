## List of things that can be added to the language

### !!! Arrays !!!

### Multi line comments
/* 
I am a multi line comment
*/

### % operator - DONE
Rest of division

### Operator that gets just the whole part of the result

### Better error reporting
Add column to the error message - DONE

### Ternary operator ?:

### Import/packages

### Throw runtime error when trying to access uninitialized variables
```mda
// No initializers.
var a;
var b;

a = "assigned";
print a; // OK, was assigned first.

print b; // Error!
```

### Foreach
```mda
foreach (var number in numbers) {}
```
Above code could be desugarized in something like
```mda
for (var i = 0; i < numbers.length; i++) {
    var number = numbers[i]; // or for performance, declare number before for and just reassign
    ...body
}
```

### Break / continue in loops

### More syntactic sugar
Examples:
```plaintext
** on numbers - power - 2 ** 3 = 8
** on strings - repeat times - "me" ** 2 = "meme"
+= -> a += 2 -> a = a + 2 - DONE
-= -> a -= 2 -> a = a - 2 - DONE
*= -> a *= 2 -> a = a * 2 - DONE
/= -> a /= 2 -> a = a / 2 - DONE
%= -> a %= 2 -> a = a % 2 - DONE
++ -> a++ -> a = a + 1 - DONE
-- -> a-- -> a = a - 1 - DONE
```

### Try/catch/finally

### Class getters and setters
```mda
class Person {
    var name;
    var age;

    get isAdult {
        return age >= 18;
    }

    set isAdult(value) {
        if (value) {
            age = 18;
        } else {
            age = 17;
        }
    }
}
```

### Static methods
```mda
class Math {
    static sum(a, b) {
        return a + b;
    }
}
```

### Static variables
```mda
class Math {
    static PI = 3.14159265359;
}
```

### Static classes
```mda
static class Math {
    static sum(a, b) {
        return a + b;
    }
}
```

### Private/public modifers
```mda
class Person {
    private var name;
    public var age;
    
    public getName() {
        return name;
    }
    
    private isAdult() {
        return age >= 18;
    }
}
```
