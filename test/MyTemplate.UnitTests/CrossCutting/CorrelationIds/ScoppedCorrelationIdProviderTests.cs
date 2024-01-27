using System;
using System.Collections.Generic;
using Amazon.Lambda.SQSEvents;
using FluentAssertions;
using MyTemplate.CrossCutting.CorrelationIds;
using Xunit;

namespace MyTemplate.UnitTests.CrossCutting.CorrelationIds;

public class ScoppedCorrelationIdProviderTests
{
    private readonly ScoppedCorrelationIdProvider _uut;

    public ScoppedCorrelationIdProviderTests()
    {
        _uut = new ScoppedCorrelationIdProvider();
    }

    [Fact]
    public void CorrelationId_ShouldReturnEmptyCorrelationIdByDefault()
    {
        _uut.CorrelationId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void TryLoadFromMessageAttributes_ShouldSetCorrelationIdWhenStringValueOnGivenKeyIsAValidGuid()
    {
        // Arrange  
        const string correlationIdKey = "x-correlation-id";

        var correlationId = Guid.NewGuid();

        var messageAttributes = new Dictionary<string, SQSEvent.MessageAttribute>
        {
            [correlationIdKey] = new SQSEvent.MessageAttribute() { StringValue = correlationId.ToString() }
        };


        // Act
        _uut.TryLoadFromMessageAttributes(correlationIdKey, messageAttributes);


        // Assert
        _uut.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void TryLoadFromMessageAttributes_ShouldNotSetCorrelationIdWhenStringValueOnGivenKeyIsNotAValidGuid()
    {
        // Arrange  
        const string correlationIdKey = "x-correlation-id";

        var messageAttributes = new Dictionary<string, SQSEvent.MessageAttribute>
        {
            [correlationIdKey] = new SQSEvent.MessageAttribute() { StringValue = "not-a-guid" }
        };


        // Act
        _uut.TryLoadFromMessageAttributes(correlationIdKey, messageAttributes);


        // Assert
        _uut.CorrelationId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void TryLoadFromMessageAttributes_ShouldNotSetCorrelationIdWhenStringValueOnGivenKeyIsNull()
    {
        // Arrange  
        const string correlationIdKey = "x-correlation-id";

        var messageAttributes = new Dictionary<string, SQSEvent.MessageAttribute>
        {
            [correlationIdKey] = new SQSEvent.MessageAttribute() { StringValue = null }
        };


        // Act
        _uut.TryLoadFromMessageAttributes(correlationIdKey, messageAttributes);


        // Assert
        _uut.CorrelationId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void TryLoadFromMessageAttributes_ShouldNotSetCorrelationIdWhenStringValueOnGivenKeyIsEmpty()
    {
        // Arrange  
        const string correlationIdKey = "x-correlation-id";

        var messageAttributes = new Dictionary<string, SQSEvent.MessageAttribute>
        {
            [correlationIdKey] = new SQSEvent.MessageAttribute() { StringValue = string.Empty }
        };


        // Act
        _uut.TryLoadFromMessageAttributes(correlationIdKey, messageAttributes);


        // Assert
        _uut.CorrelationId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void TryLoadFromMessageAttributes_ShouldNotSetCorrelationIdWhenStringValueOnGivenKeyIsWhiteSpace()
    {
        // Arrange  
        const string correlationIdKey = "x-correlation-id";

        var messageAttributes = new Dictionary<string, SQSEvent.MessageAttribute>
        {
            [correlationIdKey] = new SQSEvent.MessageAttribute() { StringValue = " " }
        };


        // Act
        _uut.TryLoadFromMessageAttributes(correlationIdKey, messageAttributes);


        // Assert
        _uut.CorrelationId.Should().Be(Guid.Empty);   
    }

    [Fact]
    public void TryLoadFromMessageAttributes_ShouldNotSetCorrelationIdWhenStringValueOnGivenKeyIsNotPresent()
    {
        // Arrange  
        const string correlationIdKey = "x-correlation-id";

        var messageAttributes = new Dictionary<string, SQSEvent.MessageAttribute>();


        // Act
        _uut.TryLoadFromMessageAttributes(correlationIdKey, messageAttributes);


        // Assert
        _uut.CorrelationId.Should().Be(Guid.Empty);
    }
}
