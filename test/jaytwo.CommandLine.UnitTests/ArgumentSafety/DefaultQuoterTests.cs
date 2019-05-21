using System;
using System.Collections.Generic;
using System.Text;
using jaytwo.CommandLine.ArgumentSafety;
using Xunit;

namespace jaytwo.CommandLine.UnitTests.ArgumentSafety
{
    public class DefaultQuoterTests
    {
        [Theory]
        [InlineData("abc", "abc")]
        [InlineData("bc'd", "\"bc'd\"")]
        [InlineData("'c' 'd' 'e'", "\"'c' 'd' 'e'\"")]
        [InlineData("\"d' 'e\" \"f\"", "\"\\\"d' 'e\\\" \\\"f\\\"\"")]
        public void Quote_produces_expected_values(string rawValue, string expected)
        {
            // arrange
            var quoter = new DefaultQuoter();

            // act
            var actual = quoter.Quote(rawValue);

            // assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("abc", "abc")]
        [InlineData("bc'd", "bc'd")]
        [InlineData("'c' 'd' 'e'", "'c' 'd' 'e'")]
        [InlineData("\"d' 'e\" \"f\"", "\\\"d' 'e\\\" \\\"f\\\"")]
        public void Escape_produces_expected_values(string rawValue, string expected)
        {
            // arrange
            var quoter = new DefaultQuoter();

            // act
            var actual = quoter.Escape(rawValue);

            // assert
            Assert.Equal(expected, actual);
        }
    }
}
