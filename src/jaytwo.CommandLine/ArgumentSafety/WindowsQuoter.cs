using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.CommandLine.ArgumentSafety
{
    public class WindowsQuoter : DefaultQuoter, IQuoter
    {
        protected override string EscpaedQuote => "\"\"";

        //protected override string EscapedNewline => "^\\n";

        //protected override string EscapedLineFeed => "^\\r";
    }
}
