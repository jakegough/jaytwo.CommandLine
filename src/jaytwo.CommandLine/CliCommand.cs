using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.CommandLine
{
    public class CliCommand
    {
        public string Arguments { get; set; }

        public IDictionary<string, string> Environment { get; } = new Dictionary<string, string>();

        public int[] ExpectedExitCodes { get; set; } = new int[] { };

        public string FileName { get; set; }

        public TimeSpan? Timeout { get; set; }

        public string WorkingDirectory { get; set; }

        public override string ToString() => $"{FileName} {Arguments}".Trim();
    }
}
