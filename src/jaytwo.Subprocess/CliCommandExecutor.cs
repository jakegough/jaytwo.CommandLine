using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jaytwo.Subprocess.Exceptions;

namespace jaytwo.Subprocess
{
    public class CliCommandExecutor : ICliCommandExecutor
    {
        private static TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(30);

        private static int[] DefaultExpectedExitCodes { get; } = new[] { 0 };

        public CliCommandResult Execute(CliCommand command)
        {
            try
            {
                // i don't think we will have even a potential a deadlock situation here because it won't actually call any async code
                return Task.Run(() => InnerExecute(command, false)).Result;
            }
            catch (AggregateException ex)
            {
                // any exception in the task will be wrapped in an AggregateException, even though we're not calling async code
                var originalException = ex.Flatten().GetBaseException();
                throw originalException;
            }
        }

        public Task<CliCommandResult> ExecuteAsync(CliCommand command)
        {
            return InnerExecute(command, true);
        }

        // with wisdom from https://stackoverflow.com/questions/1145969/processinfo-and-redirectstandardoutput
        private async Task<CliCommandResult> InnerExecute(CliCommand command, bool useAsync)
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

                if (useAsync)
                {
                    await process.WaitForExitAsync(command.Timeout ?? DefaultTimeout);
                }
                else
                {
                    var waitForExitMilliseconds = (int)(command.Timeout ?? DefaultTimeout).TotalMilliseconds;
                    process.WaitForExit(waitForExitMilliseconds);
                }

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

            var result = new CliCommandResult();
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
                throw new Exceptions.TimeoutException(result);
            }

            if (!result.Success)
            {
                throw new UnexpectedExitCodeException(result);
            }

            return result;
        }
    }
}
