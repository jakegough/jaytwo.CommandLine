using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace jaytwo.CommandLine
{
    public interface ICliCommandExecutor
    {
        Task<CliResult> RunCommandAsync(CliCommand command);
    }
}
