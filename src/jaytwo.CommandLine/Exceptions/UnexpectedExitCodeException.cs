using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.CommandLine.Exceptions
{
    public class UnexpectedExitCodeException : CommandLineException
    {
        public UnexpectedExitCodeException(CliResult result)
            : base(result, GetMessage(result))
        {
        }

        private static string GetMessage(CliResult result)
        {
            return $"Unexpected Exit Code: {result.ExitCode}\n\nStandard Error: {result.StandardError}\n\nStandard Output: {result.StandardOutput}";
        }
    }
}
