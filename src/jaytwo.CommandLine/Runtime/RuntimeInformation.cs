using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace jaytwo.CommandLine.Runtime
{
    public class RuntimeInformation
    {
        private static Lazy<RuntimeInformation> _lazyCurrent = new Lazy<RuntimeInformation>(() => new RuntimeInformationProvider().GetRuntimeInformation());

        public RuntimeInformation(OSPlatform platform, Architecture processArchitecture)
        {
            Platform = platform;
            ProcessArchitecture = processArchitecture;
        }

        public static RuntimeInformation Current => _lazyCurrent.Value;

        public OSPlatform Platform { get; }

        public Architecture ProcessArchitecture { get; }
    }
}
