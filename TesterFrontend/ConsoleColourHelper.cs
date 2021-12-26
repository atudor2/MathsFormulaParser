using System;

namespace TesterFrontend
{
    /// <summary>
    /// Base class to ease the use of console colour outputting
    /// </summary>
    public abstract class ConsoleColourHelper
    {
        /// <summary>
        /// The original Console Colour
        /// </summary>
        public ConsoleColor CurrentColour { get; private set; }
        /// <summary>
        /// The new Console Colour to be applied.
        /// </summary>
        public ConsoleColor NewColour { get; private set; }

        protected ConsoleColourHelper(ConsoleColor oldColour, ConsoleColor newColour)
        {
            this.CurrentColour = oldColour;
            this.NewColour = newColour;
        }
    }

    /// <summary>
    /// Class to handle Console Background colour changes. The new colour will be set until the class is disposed which will restore the original colour
    /// </summary>
    /// <remarks>This allows for code like:
    /// <code>
    /// Console.WriteLine("I have  the original back colour");
    /// using (new ConsoleBackColourHelper(ConsoleColour.Red)) {
    ///     Console.WriteLine("I have a red background");
    /// }
    /// Console.WriteLine("I have  the original back colour again");
    /// </code>
    /// </remarks>
    public class ConsoleBackColourHelper : ConsoleColourHelper, IDisposable
    {
        public ConsoleBackColourHelper(ConsoleColor newColour)
            : base(Console.BackgroundColor, newColour)
        {
            Console.BackgroundColor = this.NewColour;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Console.BackgroundColor = this.CurrentColour;
        }
    }

    /// <summary>
    /// Defined pairs of colours for use with ConsoleForeAndBackColourHelper
    /// </summary>
    public enum ConsoleColourPairs
    {
        RedWarningWithHighlight,
        BlueHighlight,
        RedWarning
    }

    /// <summary>
    /// Class to handle Console Background & Foreground colour changes. 
    /// The new colours will be set until the class is disposed which will restore the original colours
    /// </summary>
    /// <remarks>This allows for code like:
    /// <code>
    /// using (new ConsoleForeAndBackColourHelper(ConsoleColour.Red, ConsoleColor.White)) {
    ///     ....
    /// }
    /// </code>
    /// </remarks>
    public class ConsoleForeAndBackColourHelper : IDisposable
    {
        public ConsoleForeColourHelper ForeColour { get; private set; }
        public ConsoleBackColourHelper BackColour { get; private set; }

        public static ConsoleForeAndBackColourHelper FromColourPair(ConsoleColourPairs colourPair)
        {
            switch (colourPair)
            {
                case ConsoleColourPairs.RedWarningWithHighlight:
                    return new ConsoleForeAndBackColourHelper(ConsoleColor.Red, ConsoleColor.White);
                case ConsoleColourPairs.BlueHighlight:
                    return new ConsoleForeAndBackColourHelper(ConsoleColor.Blue, ConsoleColor.White);
                case ConsoleColourPairs.RedWarning:
                    return new ConsoleForeAndBackColourHelper(Console.BackgroundColor, ConsoleColor.Red);
                default:
                    throw new ArgumentOutOfRangeException("colourPair");
            }
        }

        public ConsoleForeAndBackColourHelper(ConsoleColor backColour, ConsoleColor foreColour)
        {
            this.BackColour = new ConsoleBackColourHelper(backColour);
            this.ForeColour = new ConsoleForeColourHelper(foreColour);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // Disposes the others...
            this.BackColour.Dispose();
            this.ForeColour.Dispose();
        }
    }

    /// <summary>
    /// Class to handle Console Foreground colour changes. The new colour will be set until the class is disposed which will restore the original colour
    /// </summary>
    /// <remarks>This allows for code like:
    /// <code>
    /// Console.WriteLine("I am the original colour");
    /// using (new ConsoleForeColourHelper(ConsoleColour.Red)) {
    ///     Console.WriteLine("I am red");
    /// }
    /// Console.WriteLine("I am the original colour again");
    /// </code>
    /// </remarks>
    public class ConsoleForeColourHelper : ConsoleColourHelper, IDisposable
    {
        public ConsoleForeColourHelper(ConsoleColor newColour)
            : base(Console.ForegroundColor, newColour)
        {
            Console.ForegroundColor = this.NewColour;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Console.ForegroundColor = this.CurrentColour;
        }
    }
}
