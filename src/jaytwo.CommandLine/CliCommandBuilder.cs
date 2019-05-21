using System;
using System.Collections.Generic;
using System.Linq;
using jaytwo.CommandLine.ArgumentSafety;
using jaytwo.CommandLine.Runtime;

namespace jaytwo.CommandLine
{
    public class CliCommandBuilder
    {
        public CliCommandBuilder()
            : this(RuntimeInformation.Current)
        {
        }

        internal CliCommandBuilder(RuntimeInformation runtimeInfo)
        {
            RuntimeInfo = runtimeInfo;
            EnvironmentVariables = new Dictionary<string, string>();
            Arguments = new CliArgumentsBuilder(runtimeInfo);
        }

        public RuntimeInformation RuntimeInfo { get; }

        public CliArgumentsBuilder Arguments { get; set; }

        public IDictionary<string, string> EnvironmentVariables { get; }

        public IList<int> ExpectedExitCodes { get; set; }

        public string FileName { get; set; }

        public TimeSpan? Timeout { get; set; }

        public string WorkingDirectory { get; set; }

        public CliCommandBuilder WithRuntimeCondition(Func<RuntimeInformation, bool> runtimeCondition, Action<CliCommandBuilder> action)
        {
            var runtimeConditionMet = runtimeCondition.Invoke(RuntimeInfo);

            if (runtimeConditionMet)
            {
                action.Invoke(this);
            }

            return this;
        }

        public CliCommandBuilder AppendRawArguments(string argument)
        {
            Arguments.AppendRaw(argument);
            return this;
        }

        public CliCommandBuilder AppendRawArguments(string format, params object[] args)
        {
            var formatted = string.Format(format, args);
            return AppendRawArguments(formatted);
        }

        public CliCommandBuilder AppendArgument(string argument)
        {
            Arguments.Append(argument);
            return this;
        }

        public CliCommandBuilder AppendArgument(string format, params object[] args)
        {
            var formatted = string.Format(format, args);
            return AppendArgument(formatted);
        }

        public CliCommandBuilder WithExpectedExitCode(int expectedExitCode)
        {
            return WithExpectedExitCodes(new[] { expectedExitCode });
        }

        public CliCommandBuilder WithExpectedExitCodes(params int[] expectedExitCodes)
        {
            ExpectedExitCodes = expectedExitCodes;
            return this;
        }

        public CliCommandBuilder WithFileName(string fileName)
        {
            FileName = fileName;
            return this;
        }

        public CliCommandBuilder WithTimeout(TimeSpan? timeout)
        {
            Timeout = timeout;
            return this;
        }

        public CliCommandBuilder WithWorkingDirectory(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        public CliCommandBuilder WithEnvironmentVariable(string key, string value)
        {
            EnvironmentVariables[key] = value;
            return this;
        }

        public CliCommandBuilder WithEnvironmentVariables(IDictionary<string, string> environmentVariables)
        {
            foreach (var environmentVariable in environmentVariables)
            {
                EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
            }

            return this;
        }

        public CliCommand GetCommand()
        {
            var result = new CliCommand()
            {
                FileName = FileName,
                WorkingDirectory = WorkingDirectory,
                Arguments = Arguments.ToString(),
                Timeout = Timeout,
                ExpectedExitCodes = ExpectedExitCodes?.ToArray(),
            };

            foreach (var environmentVariable in EnvironmentVariables)
            {
                result.Environment[environmentVariable.Key] = environmentVariable.Value;
            }

            return result;
        }
    }
}
