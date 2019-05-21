using System;
using System.Collections.Generic;
using System.Text;
using jaytwo.CommandLine.ArgumentSafety;
using Moq;
using Xunit;

namespace jaytwo.CommandLine.UnitTests
{
    public class CommandLineArgumentsBuilderTests
    {
        [Fact]
        public void Append_calls_quoter()
        {
            // arrange
            var quoter = new Mock<IQuoter>();
            quoter.Setup(x => x.Quote("hello")).Returns("HELLO");

            var builder = new CliArgumentsBuilder(quoter.Object);

            // act
            builder.Append("hello");
            builder.AppendRaw(" world");

            // assert
            var result = builder.ToString();
            Assert.Equal("HELLO world", result);
        }
    }
}
