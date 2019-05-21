using System;
using System.Collections.Generic;
using System.Text;
using jaytwo.CommandLine.ArgumentSafety;
using jaytwo.CommandLine.Runtime;

namespace jaytwo.CommandLine
{
    public class CliArgumentsBuilder
    {
        private readonly StringBuilder _result;

        public CliArgumentsBuilder()
            : this(Quoter.Current)
        {
        }

        internal CliArgumentsBuilder(RuntimeInformation runtimeInfo)
            : this(new QuoterFactory(runtimeInfo).GetQuoter())
        {
        }

        internal CliArgumentsBuilder(IQuoter quoter)
        {
            ArgumentQuoter = quoter;
            _result = new StringBuilder();
        }

        public IQuoter ArgumentQuoter { get; }

        public CliArgumentsBuilder AppendRaw(string value)
        {
            _result.Append(value);
            return this;
        }

        public CliArgumentsBuilder Append(string value)
        {
            if (_result.Length > 0)
            {
                _result.Append(" ");
            }

            _result.Append(ArgumentQuoter.Quote(value));

            return this;
        }

        public CliArgumentsBuilder Clear()
        {
            _result.Clear();
            return this;
        }

        public override string ToString()
        {
            return _result.ToString();
        }
    }
}
