using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using jaytwo.CommandLine.ArgumentSafety;
using Xunit;
using Xunit.Abstractions;

namespace jaytwo.CommandLine.UnitTests.ArgumentSafety
{
    public class QuoterTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ICliCommandExecutor _cli = new CliCommandExecutor();

        public QuoterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // TODO: look at test cases from https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Process/tests/ProcessTests.cs#L1323-L1341
        [Theory]
        [InlineData("HelloWorld")]
        [InlineData("Hello World")]
        [InlineData("`singletick \"doubletick `backtick")]
        [InlineData("$dollar !exclamation &ampersand")]
        [InlineData(">greaterthan <lessthan {leftbrace }rightbrace [leftbracket ]rightbracket")]
        [InlineData(":colon ;semicolon")]
        [InlineData("line\rfeed", Skip = "can't get newline to work")]
        [InlineData("new\nline", Skip = "can't get newline to work")]
        public async Task Current_quoter_returns_expected_for_platform(string message)
        {
            // arrange
            var quoted = Quoter.Current.Quote($"console.log('{message}')");

            var command = new CliCommandBuilder()
                .WithFileName("node")
                .AppendRawArguments("-e " + quoted)
                .GetCommand();

            // (log)
            _output.WriteLine("Command: {0}", command);

            // act
            var result = await _cli.RunCommandAsync(command);

            // (log)
            _output.WriteLine("StandardOutput: {0}", result.StandardOutput?.Trim());
            _output.WriteLine("StandardError: {0}", result.StandardError?.Trim());

            // assert
            Assert.Equal(message, result.StandardOutput?.Trim());
        }
    }
}
