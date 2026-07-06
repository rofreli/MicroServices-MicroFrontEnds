using OAuth.Domain.Exceptions;
using OAuth.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace OAuth.UnitTests.Domain;

public class EmailTests
{
    [Fact]
    public void Normalizes_to_lowercase()
    {
        new Email("Rodrigo@Acme.COM").Value.Should().Be("rodrigo@acme.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("a@b")]
    [InlineData("a@@b.com")]
    public void Rejects_invalid_email(string input)
    {
        var act = () => new Email(input);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equality_is_by_value_case_insensitive()
    {
        new Email("a@b.com").Should().Be(new Email("A@B.COM"));
    }
}
