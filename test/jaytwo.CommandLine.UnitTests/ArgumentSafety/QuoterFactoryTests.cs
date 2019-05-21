using System;
using System.Collections.Generic;
using System.Text;
using jaytwo.CommandLine.ArgumentSafety;
using jaytwo.CommandLine.Runtime;
using Moq;
using Xunit;

namespace jaytwo.CommandLine.UnitTests.ArgumentSafety
{
    public class QuoterFactoryTests
    {
        [Fact]
        public void GetQuoter_returns_WindowsQuoter_when_Platform_is_Windows()
        {
            // arrange
            var runtimeInfo = new RuntimeInformation(OSPlatform.Windows, Architecture.Unknown);
            var factory = new QuoterFactory(runtimeInfo);

            // arrange
            var quoter = factory.GetQuoter();

            // assert
            Assert.IsType<WindowsQuoter>(quoter);
        }

        [Fact]
        public void GetQuoter_returns_DefaultQuoter_when_Platform_is_Linux()
        {
            // arrange
            var runtimeInfo = new RuntimeInformation(OSPlatform.Linux, Architecture.Unknown);
            var factory = new QuoterFactory(runtimeInfo);

            // arrange
            var quoter = factory.GetQuoter();

            // assert
            Assert.IsType<DefaultQuoter>(quoter);
        }

        [Fact]
        public void GetQuoter_returns_LinuxQuoter_when_Platform_is_OSX()
        {
            // arrange
            var runtimeInfo = new RuntimeInformation(OSPlatform.OSX, Architecture.Unknown);
            var factory = new QuoterFactory(runtimeInfo);

            // arrange
            var quoter = factory.GetQuoter();

            // assert
            Assert.IsType<DefaultQuoter>(quoter);
        }
    }
}
