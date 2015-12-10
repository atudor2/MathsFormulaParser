using Alistair.Tudor.MathsFormulaParser.Internal.Functions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Functions
{
    /// <summary>
    /// Concrete form of a standard function
    /// </summary>
    internal class StandardFunction : FormulaFunction
    {
        public StandardFunction(string functionName, FormulaCallbackFunction callbackFunction, int requiredNumberOfArguments) :
            base(functionName, callbackFunction, requiredNumberOfArguments)
        {
        }
    }
}
