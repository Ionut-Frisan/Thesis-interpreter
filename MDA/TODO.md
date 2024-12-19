## List of things that can be added to the language

### !!! Arrays !!!

### Multi line comments
/* 
I am a multi line comment
*/

### % operator
Rest of division

### Operator that gets just the whole part of the result

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
+= -> a += 2 -> a = a + 2
-= -> a -= 2 -> a = a - 2
++ -> a++ -> a = a + 1
-- -> a-- -> a = a - 1
```

