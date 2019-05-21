using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jaytwo.CommandLine.Exceptions;

namespace jaytwo.CommandLine
{
    public class CliCommandExecutor : ICliCommandExecutor
    {
        private static TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(30);

        private static int[] DefaultExpectedExitCodes { get; } = new[] { 0 };

        // with wisdom from https://stackoverflow.com/questions/1145969/processinfo-and-redirectstandardoutput
        public async Task<CliResult> RunCommandAsync(CliCommand command)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = false; // false if the process should be created directly from the executable file (must be false for `RedirectStandardOutput = true` and `RedirectStandardError = true`)
            processStartInfo.RedirectStandardOutput = true; // true if output should be written to System.Diagnostics.Process.StandardOutput
            processStartInfo.RedirectStandardError = true; // true if error output should be written to System.Diagnostics.Process.StandardError
            processStartInfo.CreateNoWindow = true; // true if the process should be started without creating a new window to contain it

            if (command.Environment != null)
            {
#if NETFRAMEWORK
                var startInfoEnvironmentVariables = processStartInfo.EnvironmentVariables;
#else
                var startInfoEnvironmentVariables = processStartInfo.Environment;
#endif

                foreach (var environmentVariable in command.Environment)
                {
                    startInfoEnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
                }
            }

            processStartInfo.WorkingDirectory = command.WorkingDirectory;
            processStartInfo.FileName = command.FileName;
            processStartInfo.Arguments = command.Arguments;

            var standardOutputBuilder = new StringBuilder();
            var standardErrorBuilder = new StringBuilder();

            int exitCode;
            bool timedOut = false;
            TimeSpan duration;
            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.ErrorDataReceived += (sender, e) => { standardErrorBuilder.AppendLine(e.Data); }; // Occurs when an application writes to its redirected System.Diagnostics.Process.StandardError stream.
                process.OutputDataReceived += (sender, e) => { standardOutputBuilder.AppendLine(e.Data); }; // Occurs each time an application writes a line to its redirected System.Diagnostics.Process.StandardOutput stream.
                process.EnableRaisingEvents = true; // true if the System.Diagnostics.Process.Exited event should be raised when the associated process is terminated (through either an exit or a call to System.Diagnostics.Process.Kill);

                var stopwatch = Stopwatch.StartNew();
                process.Start();
                process.BeginOutputReadLine(); // Requires RedirectStandardOutput = true; Begins asynchronous read operations on the redirected System.Diagnostics.Process.StandardOutput stream of the application.
                process.BeginErrorReadLine(); // Requires RedirectStandardError = true; Begins asynchronous read operations on the redirected System.Diagnostics.Process.StandardError stream of the application.

                await process.WaitForExitAsync(command.Timeout ?? DefaultTimeout);
                process.Refresh();
                if (!process.HasExited)
                {
                    try
                    {
                        // if process has exited between checking and trying to kill, an exception will happen
                        process.Kill();
                        timedOut = true;
                    }
                    catch
                    {
                    }

                    process.WaitForExit();
                    stopwatch.Stop();
                }

                exitCode = process.ExitCode;
                duration = stopwatch.Elapsed;
            }

            var result = new CliResult();
            result.Command = command;
            result.Duration = duration;
            result.StandardError = standardErrorBuilder.ToString();
            result.StandardOutput = standardOutputBuilder.ToString();
            result.ExitCode = exitCode;
            result.TimedOut = timedOut;

            var expectedExitCodes = (command.ExpectedExitCodes ?? new int[] { }).Any()
                ? command.ExpectedExitCodes
                : DefaultExpectedExitCodes;

            result.Success = !timedOut && expectedExitCodes.Contains(exitCode);

            if (result.TimedOut)
            {
                throw new CommandTimeoutException(result);
            }

            if (!result.Success)
            {
                throw new UnexpectedExitCodeException(result);
            }

            return result;
        }
    }
}
