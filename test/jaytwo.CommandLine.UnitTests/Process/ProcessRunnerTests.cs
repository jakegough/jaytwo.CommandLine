using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using jaytwo.CommandLine.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace jaytwo.CommandLine.UnitTests.Process
{
    public class ProcessRunnerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ICliCommandExecutor _cli = new CliCommandExecutor();

        public ProcessRunnerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(15)]
        [InlineData(31)]
        [InlineData(63)]
        [InlineData(127)]
        [InlineData(255)]
        public async Task Cli_captures_exit_code(int exitCode)
        {
            // arrange
            var command = new CliCommandBuilder()
                .WithFileName("node")
                .WithArguments("-e", $"process.exit({exitCode})")
                .WithExpectedExitCodes(exitCode)
                .GetCommand();

            // act
            var result = await _cli.RunCommandAsync(command);

            // (log)
            _output.WriteLine("Command: {0}", result.Command);
            _output.WriteLine("ExitCode: {0}", result.ExitCode);
            _output.WriteLine("StandardOutput: {0}", result.StandardOutput?.Trim());
            _output.WriteLine("StandardError: {0}", result.StandardError?.Trim());

            // assert
            Assert.Equal(exitCode, result.ExitCode);
        }

        [Fact]
        public async Task Cli_processes_input_arguments_and_captures_standard_output()
        {
            // arrange
            var message = "hello world";

            var command = new CliCommandBuilder()
                .WithFileName("node")
                .WithArgument("-e")
                .WithArgument("console.log('{0}')", message)
                .GetCommand();

            // act
            var result = await _cli.RunCommandAsync(command);

            // (log)
            _output.WriteLine("Command: {0}", result.Command);
            _output.WriteLine("ExitCode: {0}", result.ExitCode);
            _output.WriteLine("StandardOutput: {0}", result.StandardOutput?.Trim());
            _output.WriteLine("StandardError: {0}", result.StandardError?.Trim());

            // assert
            Assert.Equal(message, result.StandardOutput?.Trim());
            Assert.Empty(result.StandardError?.Trim());
        }

        [Theory]
        [InlineData("abc", null)]
        [InlineData("abc def", null)]
        [InlineData("abc\"def", null)]
        [InlineData("abc'def", null)]
        [InlineData(@"abc\def", null)]
        [InlineData("abc\ndef", "abc[newline]def")]
        [InlineData("abc\rdef", "abc[newline]def")]
        [InlineData("`-=~!@#$%^&*()_+", null)]
        [InlineData("[];,./{}|:<>?", null)]
        public async Task Cli_arguments_work_with_special_characters(string message, string expectedOutput)
        {
            // arrange
            var messageReadyForConsoleLog = message
                .Replace("\\", "\\\\") // slashes first so we don't redo them as part of escaping quotes or newlines
                .Replace("'", @"\'") // because we're already inside single quotes
                .Replace("\n", @"\n") // we have to pass newlines as strings to be evaluated inside the quotes
                .Replace("\r", @"\r");

            var command = new CliCommandBuilder()
                .WithFileName("node")
                .WithArgument("-e")
                .WithArgument("console.log('{0}')", messageReadyForConsoleLog)
                .GetCommand();

            // (log)
            _output.WriteLine("Command: {0}", command);

            // act
            var result = await _cli.RunCommandAsync(command);

            // (log)
            _output.WriteLine("StandardOutput: {0}", result.StandardOutput?.Trim());

            // assert
            expectedOutput = expectedOutput?.Replace("[newline]", Environment.NewLine);

            Assert.Equal(expectedOutput ?? message, result.StandardOutput?.Trim());
        }

        [Fact]
        public async Task Cli_processes_input_arguments_and_captures_standard_error()
        {
            // arrange
            var message = "hello world";

            var command = new CliCommandBuilder()
                .WithFileName("node")
                .WithArgument("-e")
                .WithArgument("console.error('{0}')", message)
                .GetCommand();

            // act
            var result = await _cli.RunCommandAsync(command);

            // (log)
            _output.WriteLine("Command: {0}", result.Command);
            _output.WriteLine("ExitCode: {0}", result.ExitCode);
            _output.WriteLine("StandardOutput: {0}", result.StandardOutput?.Trim());
            _output.WriteLine("StandardError: {0}", result.StandardError?.Trim());

            // assert
            Assert.Empty(result.StandardOutput?.Trim());
            Assert.Equal(message, result.StandardError?.Trim());
        }

        [Theory]
        [InlineData(5000, 100, true)]
        [InlineData(100, 5000, false)]
        public async Task Cli_processes_timeout(double sleepSeconds, double timeoutSeconds, bool expectedTimedOut)
        {
            // arrange
            var command = new CliCommandBuilder()
                .WithFileName("node")
                .WithArgument("-e")
                .WithArgument("setTimeout(function() {{ console.log('exiting as scheduled...'); process.exit(123); }}, {0});", sleepSeconds)
                .WithTimeout(TimeSpan.FromMilliseconds(timeoutSeconds))
                .GetCommand();

            // act
            var result = await _cli.RunCommandAsync(command, true);

            // (log)
            _output.WriteLine("Command: {0}", result.Command);
            _output.WriteLine("ExitCode: {0}", result.ExitCode);
            _output.WriteLine("Duration: {0:n0}ms", result.Duration.TotalMilliseconds);
            _output.WriteLine("StandardOutput: {0}", result.StandardOutput?.Trim());
            _output.WriteLine("StandardError: {0}", result.StandardError?.Trim());

            // assert
            Assert.Equal(expectedTimedOut, result.Duration.TotalMilliseconds > timeoutSeconds);
            Assert.Equal(expectedTimedOut, result.TimedOut);
        }
    }
}
