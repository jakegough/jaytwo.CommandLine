using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jaytwo.CommandLine.ArgumentSafety
{
    public class DefaultQuoter : Quoter, IQuoter
    {
        private readonly Regex _quoteableCharactersRegex;
        private readonly Regex _specialCharactersRegex;

        public DefaultQuoter(string quoteableCharactersPattern = "['\"`\\s]", string specialCharactersPattern = "['\"`\\s]")
        {
            _quoteableCharactersRegex = new Regex(quoteableCharactersPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            _specialCharactersRegex = new Regex(specialCharactersPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        protected override bool ShouldQuote(string value)
        {
            return (value != null) && _quoteableCharactersRegex.IsMatch(value);
        }

        protected override bool ShouldEscape(string value)
        {
            return (value != null) && _specialCharactersRegex.IsMatch(value);
        }
    }
}
