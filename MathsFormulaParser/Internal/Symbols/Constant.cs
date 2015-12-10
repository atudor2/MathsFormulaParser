namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols
{
    /// <summary>
    /// Represents a constant
    /// </summary>
    internal class Constant
    {
        public Constant(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the constant
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the constant
        /// </summary>
        public double Value { get; }
    }
}
