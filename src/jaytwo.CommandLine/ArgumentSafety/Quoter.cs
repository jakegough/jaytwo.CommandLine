using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace jaytwo.CommandLine.ArgumentSafety
{
    public abstract class Quoter : IQuoter
    {
        private static readonly Lazy<IQuoter> _lazyCurrent = new Lazy<IQuoter>(() => new QuoterFactory().GetQuoter());

        public static IQuoter Current => _lazyCurrent.Value;

        protected virtual string EscpaedQuote => "\\\"";

        protected virtual string EscapedNewline => "\\\\n";

        protected virtual string EscapedLineFeed => "\\\\r";

        public virtual string Escape(string value)
        {
            if (ShouldEscape(value))
            {
                return value?
                    .Replace("\"", EscpaedQuote)
                    .Replace("\n", EscapedNewline)
                    .Replace("\r", EscapedLineFeed);
            }
            else
            {
                return value;
            }
        }

        public virtual string Quote(string value)
        {
            if (ShouldQuote(value))
            {
                return "\"" + Escape(value) + "\"";
            }
            else
            {
                return value;
            }
        }

        protected abstract bool ShouldQuote(string value);

        protected abstract bool ShouldEscape(string value);
    }
}
