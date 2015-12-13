This is the formula parser library README

================
Known Issues:
================
1) "Failure Point" messages (e.g.:
    
    1 + 1 +
    ------^

    Sometimes don't correctly 'point' to the problem token

2) Error messages can be too techincal for the library user (e.g. they don't need to know about invalid RPN Evaluator token stack count)

================
Usage:
================

var manager = new FormulaManager("log(A,2) + 2"); // Create a formula manager
// ... you can define custom functions, constants etc ....
var evaluator = manager.CreateFormulaEvaluator(); // Parse the formula and create an evaluator
evaluator.OptimiseFormula(<Optimisation Level>); // Optimise the parsed formula (see further down)
evaluator.SetVariableMap(<variable dictionary>); // Set the variable values
var result = evaluator.GetResult(); // Calc the result

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
            ((-b + sqrt(b**2 - 4*a*c))/(2 * a))
            A = 1
            B = -3
            C = -4

            is compiled to something like this:

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
How it works:
================

Stream of chracters is input to the Lexer which converts each character or set of characters into LexicalTokens 
(e.g.: 1 + 2 + 3 => <NUMBER><SPACE><OPERATOR><NUMBER><SPACE><OPERATOR><NUMBER>). This is then fed into the Parser which validates the syntax
and converts it from Infix Notation to "Reverse Polish Notation" (RPN) via the "Shunting Yard" Algorithm. The main benefits of RPN is that executing
a formula like 1 + (2 + 3) gets converted to 1 2 3 + + 

================
Performance:
================

I have measured average performance over 1 million calls to be between 0.0005-0.01ms per evaluation depending on the formula complexity

================
Builtin Operators, functions and constants:
================

