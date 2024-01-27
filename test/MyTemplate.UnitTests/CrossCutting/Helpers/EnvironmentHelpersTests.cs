using System;
using FluentAssertions;
using MyTemplate.CrossCutting.Helpers;
using Xunit;

namespace MyTemplate.UnitTests.CrossCutting.Helpers;

public class EnvironmentHelpersTests : IDisposable
{
    [Fact]
    public void IsLocalEnvironment_ShouldReturnTrueWhenExecutingEnvironmentIsLocal()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", "local");


        // Act
        var result = EnvironmentHelpers.IsLocalEnvironment;


        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLocalEnvironment_ShouldReturnFalseWhenExecutingEnvironmentIsNotLocal()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", "not-local");


        // Act
        var result = EnvironmentHelpers.IsLocalEnvironment;


        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsLocalEnvironment_ShouldReturnFalseWhenExecutingEnvironmentIsNotSet()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", null);


        // Act
        var result = EnvironmentHelpers.IsLocalEnvironment;


        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsTestingEnvironment_ShouldReturnTrueWhenExecutingEnvironmentIsTest()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", "test");


        // Act
        var result = EnvironmentHelpers.IsTestingEnvironment;
    

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTestingEnvironment_ShouldReturnFalseWhenExecutingEnvironmentIsNotTest()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", "not-test");


        // Act
        var result = EnvironmentHelpers.IsTestingEnvironment;
    

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsTestingEnvironment_ShouldReturnFalseWhenExecutingEnvironmentIsNotSet()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", null);


        // Act
        var result = EnvironmentHelpers.IsTestingEnvironment;
    

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("dev")]
    [InlineData("hom")]
    [InlineData("prod")]

    public void IsObservableEnvironment_ShouldReturnTrueWhenExecutingEnvironmentIsNotLocalAndNotTest(string executingEnvironment)
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", executingEnvironment);
    

        // Act
        var result = EnvironmentHelpers.IsObservableEnvironment;


        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsObservableEnvironment_ShouldReturnFalseWhenExecutingEnvironmentIsLocal()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", "local");


        // Act
        var result = EnvironmentHelpers.IsObservableEnvironment;
    

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsObservableEnvironment_ShouldReturnFalseWhenExecutingEnvironmentIsTest()
    {
        // Arrange
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", "test");


        // Act
        var result = EnvironmentHelpers.IsObservableEnvironment;
    

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("EXECUTING_ENVIRONMENT", null);
    }
}
