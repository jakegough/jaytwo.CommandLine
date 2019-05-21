using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public RuntimeInformation RuntimeInfo { get; }

        public IList<string> Arguments { get; set; } = new List<string>();

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

        public CliCommandBuilder WithArguments(params string[] arguments)
        {
            foreach (var argument in arguments)
            {
                WithArgument(argument);
            }

            return this;
        }

        public CliCommandBuilder WithArgument(string argument)
        {
            Arguments.Add(argument);
            return this;
        }

        public CliCommandBuilder WithArgument(string format, params object[] args)
        {
            var formatted = string.Format(format, args);
            return WithArgument(formatted);
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
                Timeout = Timeout,
                ExpectedExitCodes = ExpectedExitCodes?.ToArray(),
            };

            foreach (var argument in Arguments)
            {
                result.Arguments.Add(argument);
            }

            foreach (var environmentVariable in EnvironmentVariables)
            {
                result.Environment[environmentVariable.Key] = environmentVariable.Value;
            }

            return result;
        }
    }
}
