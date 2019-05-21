using System;
using System.Threading.Tasks;
using jaytwo.CommandLine.Exceptions;

namespace jaytwo.CommandLine
{
    public static class ICliCommandExecutorExtensions
    {
        public static async Task<CliResult> RunCommandAsync(this ICliCommandExecutor executor, CliCommand command, bool suppressCommandLineExceptions)
        {
            CliResult result;

            if (suppressCommandLineExceptions)
            {
                try
                {
                    result = await executor.RunCommandAsync(command);
                }
                catch (CommandLineException ex)
                {
                    result = ex.Result;
                }
            }
            else
            {
                result = await executor.RunCommandAsync(command);
            }

            return result;
        }
    }
}
