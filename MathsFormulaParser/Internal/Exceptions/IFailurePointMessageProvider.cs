using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    internal interface IFailurePointMessageProvider
    {
        /// <summary>
        /// Position along input where failure occurred
        /// </summary>
        long FailurePosition { get; }

        /// <summary>
        /// Tries to create a 'failure point' message showing the user where exactly an error occurred on the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string TryMakeFailurePointMessage(string input);
    }
}
