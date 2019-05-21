using System;
using System.Collections.Generic;
using System.Text;
using jaytwo.CommandLine.Runtime;

namespace jaytwo.CommandLine.ArgumentSafety
{
    internal class QuoterFactory
    {
        private readonly RuntimeInformation _runtimeInfo;

        public QuoterFactory()
            : this(RuntimeInformation.Current)
        {
        }

        public QuoterFactory(RuntimeInformation runtimeInfo)
        {
            _runtimeInfo = runtimeInfo;
        }

        public IQuoter GetQuoter()
        {
            if (_runtimeInfo.Platform == OSPlatform.Windows)
            {
                return new WindowsQuoter();
            }
            else
            {
                return new DefaultQuoter();
            }
        }
    }
}
