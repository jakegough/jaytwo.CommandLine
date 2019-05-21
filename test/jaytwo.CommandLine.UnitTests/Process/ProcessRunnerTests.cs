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
                .AppendRawArguments("-e process.exit({0})", exitCode)
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
                .AppendRawArguments("-e \"console.log('{0}')\"", message)
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

        [Fact]
        public async Task Cli_processes_input_arguments_and_captures_standard_error()
        {
            // arrange
            var message = "hello world";

            var command = new CliCommandBuilder()
                .WithFileName("node")
                .AppendRawArguments("-e \"console.error('{0}')\"", message)
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
                .AppendRawArguments("-e \"setTimeout(function() {{ console.log('exiting as scheduled...'); process.exit(123); }}, {0});\"", sleepSeconds)
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
