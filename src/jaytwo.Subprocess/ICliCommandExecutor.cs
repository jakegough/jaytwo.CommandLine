using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace jaytwo.Subprocess
{
    public interface ICliCommandExecutor
    {
        CliCommandResult Execute(CliCommand command);

        Task<CliCommandResult> ExecuteAsync(CliCommand command);
    }
}
