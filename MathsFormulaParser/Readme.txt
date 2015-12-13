This is the formula parser library README

================
Known Issues:
================
1) "Failure Point" messages (e.g.:
    
    1 + 1 +
    ------^

    Sometimes don't correctly 'point' to the problem token. THey can be 1 or 2 tokens off

2) Error messages can be too technical for the library user (e.g. they don't need to know about invalid RPN Evaluator token stack state, just that 
   evaluation failed)

3) Callback functions are ID'd by name only and arity is fixed! Therefore you cannot have log(x) and log(x,y) as only one will be used

================
Usage (Example):
================

var manager = new FormulaManager("log(A,2) + 2"); // Create a formula manager
// ... you can define custom functions, constants etc ....
var evaluator = manager.CreateFormulaEvaluator(); // Parse the formula and create an evaluator
evaluator.OptimiseFormula(<Optimisation Level>); // Optimise the parsed formula (see further down)
evaluator.SetVariableMap(<variable dictionary>); // Set the variable values
var result = evaluator.GetResult(); // Calc the result

================
Usage:
================

Custom constants and functions can be added via the AddCustomConstant() and AddCustomCallbackFunction() methods on FormulaManager. 
THESE MUST BE CALLED BEFORE CALLING CreateFormulaEvaluator()

Custom constants and functions will have a "_" placed automatically before the name to prevent conflicts. E.g. superDuper() will become _superDuper()

NB: DO NOT ADD THE _ to the name at registration. It will be rejected.

Variables must start with a letter and then can follow with any number, letter and _. Functions, constants and variables are treated always as uppercase
(so A and a are the same variable).

================
How it works:
================

A stream of chracters is input to the Lexer which converts each character or set of characters into LexicalTokens 
(e.g.: 1 + 2 + 3 => <NUMBER><SPACE><OPERATOR><NUMBER><SPACE><OPERATOR><NUMBER>). This is then fed into the Parser which validates the syntax
and converts it from Infix Notation to "Reverse Polish Notation" (RPN) tokens via the "Shunting Yard" Algorithm. These tokens can then be optimised
and eventually evaluated to produce a result by an Evaluator.

The main benefits of RPN is that executing a formula like 1 + (2 + 3) gets converted to 1 2 3 + +  and can then be evaluated from 
left->right using a stack without having to worry about sub expression precedence etc as everything in RPN has already 
been setup in the correct evaluation order.

================
Formula Optimisation:
================

There are currently 3 levels of optimisation:

1) NONE: Take a guess at what this does :)

2) BASIC: This will pre-evaluate constant expressions E.g.: (pow(1 + 2, 2)) / A will be optimised to 9 / A. 
          This increases performance due to less tokens to iterate. THe down side is that any expression involving a 
          variable cannot be optimised, so the amount of optimisation available depends on the formula.

3) COMPILED: This is currently the most optimised and performant level. THis will firts perform a BASIC optimisation and then it
            will dynamically generate code on the fly for direct function calls. This is equivilent to creating the formula in code 
            as a series of function calls.
            E.g.: 
            ((-b + sqrt(b**2 - 4*a*c))/(2 * a)) is compiled to something like this:

    .Call Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl.BuiltInMathsSymbols.Divide(.NewArray System.Double[] {
        .Call Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl.BuiltInMathsSymbols.Add(.NewArray System.Double[] {
            .Call Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl.BuiltInMathsSymbols.UnaryNegative(.NewArray System.Double[] {
                    .Call .Constant<Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers.IIVariableResolver>(Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.CompiledFormulaEvaluator).ResolveVariable("B")
            }),
            .Call .Constant<Alistair.Tudor.MathsFormulaParser.Internal.Functions.Impl.MathFunctions+<>c__DisplayClass3_0>(Alistair.Tudor.MathsFormulaParser.Internal.Functions.Impl.MathFunctions+<>c__DisplayClass3_0).<Make1ArityThunk>b__0(.NewArray System.Double[] {
                    .Call Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl.BuiltInMathsSymbols.Subtract(.NewArray System.Double[] {
                        .Call Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl.BuiltInMathsSymbols.Power(.NewArray System.Double[] {
                            .Call .Constant<Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers.IIVariableResolver>(Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.CompiledFormulaEvaluator).ResolveVariable("B")
                                ,
                                2D
                            }),
                            .Call Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl.BuiltInMathsSymbols.Product(.NewArray System.Double[] {
                                4D,
                                .Call .Constant<Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers.IIVariableResolver>(Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.CompiledFormulaEvaluator).ResolveVariable("C")
                                })
                        })
                    })
            }),
        .Call Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl.BuiltInMathsSymbols.Product(.NewArray System.Double[] {
                2D,
                .Call .Constant<Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers.IIVariableResolver>(Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.CompiledFormulaEvaluator).ResolveVariable("A")
            })
    })

    or in a easier form:

    Divide(Add(-B, SQRT(Subtract(Power(B, 2), Multiply(4, Multiply(A, C))), Multiply(2, A))


================
Performance:
================

I have measured average performance over 1 million calls to be between 0.0005-0.01ms per evaluation depending on the formula complexity.
Memory usage remained stable over 11 million repeated calls, however using COMPILED optimisation resulted in 'bubbles' of memory usage due
to the delay of the GC collecting dead DynamicMethods. I am assuming that is because they were promoted to the Gen2 of the GC and are only collected 
much further down the line. However, no signs of leaks were seen

================
Built-in Constants:
================
PI : 3,14159265358979
EU : 2,71828182845905

================
Built-in Operators:
================
-(a) => Unary Negative
~(a) => Unary Bitwise NOT
+(a, b) => Add
&(a, b) => Bitwise AND
<<(a, b) => Bitshift Left
>>(a, b) => Bitshift Right
/(a, b) => Divide
//(a, b) => Integer Divide
%(a, b) => Modulus
|(a, b) => Bitwise OR
**(a, b) => Power (x ** 2)
*(a, b) => Mulitply
-(a, b) => Subtract
^(a, b) => Bitwise XOR

================
Built-in Functions:
================
deg2rad(a) => Convert Degrees to Radians
getbit(a, b) => Gets the bit at position b of integer a (e.g. GetBit(5,1) == 1)
rad2deg(a) => Convert Radians to Degrees
ln(a) => Natural Log
================
<Below functions are imported from System.Math and potentially remapped to deal with arity issues (see above)>
================
acos(a)
asin(a)
atan(a)
atan2(a, b)
ceiling(a)
cos(a)
cosh(a)
floor(a)
sin(a)
tan(a)
sinh(a)
tanh(a)
round(a)
truncate(a)
sqrt(a)
log10(a)
exp(a)
pow(a, b)
ieeeremainder(a, b)
abs(a)
max(a, b)
min(a, b)
log(a, b)

