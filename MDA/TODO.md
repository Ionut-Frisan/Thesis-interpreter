## List of things that can be added to the language

- [ ] Arrays !important!
- [ ] Multi line comments
  ```mda
  /* 
  I am a multi line comment
  */
  ```
- [x] % operator (Rest of division)
- [x] Compound assignment operators (+=, -=, *=, /=, %=)
- [x] ++ and -- operators
- [ ] Operator that gets just the whole part of the result of a division 
- [ ] Better error reporting
  - [x] Add column to the error message 
  - [ ] Better messages
  - [ ] Print underlined code snippet with error
- [ ] Ternary operator ?:
- [ ] Import/packages
- [ ] Try/catch/finally and Error class
- [ ] Throw runtime error when trying to access uninitialized variables
  ```mda
  // No initializers.
  var a;
  var b;
  
  a = "assigned";
  print a; // OK, was assigned first.
  
  print b; // Error!
  ```

- [ ] Foreach loop
  ```
  foreach (var number in numbers) {}
  ```
    Desugarized example:
  ```
  for (var i = 0; i < numbers.length; i++) {
      var number = numbers[i]; // or for performance, declare number before for and just reassign
      ...body
  }
  ```
- [ ] Break / continue in loops
- [ ] More syntactic sugar
[ ] Static classes
```
static class Math {
  static sum(a, b) {
      return a + b;
  }
}
```
- [ ] Static methods
  ```
  class Math {
      static sum(a, b) {
          return a + b;
      }
  }
    ```
- [ ] Static properties
    ```mda
    class Math {
        static PI = 3.14159265359;
    }
    ```
- [ ] Private/public access modifiers
  ```
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
- [ ] Class getters and setters
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
    }```
