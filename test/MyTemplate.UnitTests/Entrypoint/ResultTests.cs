using System;
using FluentAssertions;
using MyTemplate.Entrypoint;
using Xunit;

namespace MyTemplate.UnitTests.Entrypoint;

public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = Result.Success();   
    
    
        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IsFailure_ShouldReturnFalseWhenResultIsSuccess()
    {
        // Arrange & Act
        var result = Result.Success();   
    

        // Assert
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Failure_ShouldReturnFailureResult()
    {
        // Arrange & Act
        var result = Result.Failure(new Exception());   
    

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void IsFailure_ShouldReturnTrueWhenResultIsFailure()
    {
        // Arrange & Act
        var result = Result.Failure(new Exception());   
    

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ImplicitOperatorBool_ShouldReturnTrueWhenResultIsSuccess()
    {
        // Arrange & Act
        bool result = Result.Success();   
    

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ImplicitOperatorBool_ShouldReturnFalseWhenResultIsFailure()
    {
        // Arrange & Act
        bool result = Result.Failure(new Exception());   
    

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ImplicitOperatorResult_ShouldReturnFailureResult()
    {
        // Arrange & Act
        Result result = new Exception();   
    

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Error_ShouldReturnErrorWhenResultIsFailure()
    {
        // Arrange
        var error = new Exception();
        var result = Result.Failure(error);
    

        // Act
        var resultError = result.Error;
    

        // Assert
        resultError.Should().Be(error);
    }
}
