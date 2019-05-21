using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.CommandLine.ArgumentSafety
{
    public interface IQuoter
    {
        string Escape(string value);

        string Quote(string value);
    }
}
