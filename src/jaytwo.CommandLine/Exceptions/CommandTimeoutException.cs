using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.CommandLine.Exceptions
{
    public class CommandTimeoutException : CommandLineException
    {
        public CommandTimeoutException(CliResult result)
            : base(result, GetMessage(result))
        {
        }

        private static string GetMessage(CliResult result)
        {
            return $"Command timed out.\n\nStandard Error: {result.StandardError}\n\nStandard Output: {result.StandardOutput}";
        }
    }
}
