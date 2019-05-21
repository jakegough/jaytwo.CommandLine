using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.CommandLine
{
    public class CliResult
    {
        public CliCommand Command { get; set; }

        public TimeSpan Duration { get; set; }

        public int ExitCode { get; set; }

        public string StandardError { get; set; }

        public string StandardOutput { get; set; }

        public bool Success { get; set; }

        public bool TimedOut { get; set; }

        public override string ToString() => string.Format("({0} {1:n0} ms) {2}", TimedOut ? "TIMEOUT" : Success ? "OK" : "FAIL", Duration.Milliseconds, Command);
    }
}
