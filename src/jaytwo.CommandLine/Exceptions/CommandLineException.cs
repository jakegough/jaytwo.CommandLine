using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.CommandLine.Exceptions
{
    public class CommandLineException : Exception
    {
        public CommandLineException(CliResult result, string message)
            : base(message)
        {
            Result = result;
        }

        public CliResult Result { get; }
    }
}
