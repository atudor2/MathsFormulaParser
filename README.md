# Maths Formula Parser

Rediscovered this project from 7 years ago on an old HDD, converted it to a .Net 6 project and published to GH for a bit of fun - quality definitely not guaranteed :)

## What is this?

7 years ago I wanted to play around with creating a parser and lexer for basic maths formulae.
This project allowed me to play with the [Shunting-yard algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm) and [Reverse Polish notation](https://en.wikipedia.org/wiki/Reverse_Polish_notation) to create a stack based evaluator.

Scope creep resulted in a basic formula optimiser and compiler to generate code at runtime for better performance being added.

## What has been changed

The solution is pretty much as-is except for:

* The conversion of the solution & project files to .Net 6
* Removal of references to other libraries that are not fit for publication
* Correction of any other compilation errors
* Enabling of null checks in the main library with any associated warnings resolved
* Fixing of any egregious typos
