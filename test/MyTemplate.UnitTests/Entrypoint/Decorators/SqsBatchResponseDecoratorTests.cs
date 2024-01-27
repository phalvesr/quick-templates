using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using FluentAssertions;
using MyTemplate.CrossCutting.CorrelationIds;
using MyTemplate.Entrypoint;
using MyTemplate.Entrypoint.Decorators;
using MyTemplate.Entrypoint.Handlers.Abstractions;
using NSubstitute;
using Serilog;
using Xunit;

namespace MyTemplate.UnitTests.Entrypoint.Decorators;

public class SqsBatchResponseDecoratorTests
{
    private readonly CancellationToken DeterministicCancellationToken = new CancellationToken();
    private readonly ILogger _logger;
    private readonly IScoppedCorrelationIdProvider _correlationIdProvider;

    private readonly SqsBatchResponseDecorator _uut;

    public SqsBatchResponseDecoratorTests()
    {
        _logger = Substitute.For<ILogger>();
        _correlationIdProvider = Substitute.For<IScoppedCorrelationIdProvider>();

        _uut = new SqsBatchResponseDecorator(_logger, _correlationIdProvider);
    }

    [Fact]
    public void Decorate_ShouldAddMessageIdToFailedBatchItemsWhenExceptionIsThrownByTheHandler()
    {
        // Arrange
        var handler = Substitute.For<IHandler<POCO>>();

        handler.When(x => x.Handle(Arg.Any<POCO>()))
            .Do(x => throw new Exception("Thrown by test"));


        var messageId = Guid.NewGuid().ToString();
        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = messageId,
                    Body = "{}"
                }
            }
        };


        // Act
        var result = _uut.Decorate(handler, sqsEvent);
    

        // Assert
        result.BatchItemFailures.Single().ItemIdentifier.Should().Be(messageId);
    }

    [Fact]
    public void Decorate_ShouldNotAddMessageIdFailureBatchItemsWhenNoExceptionIsThrownByTheHandler()
    {
        // Arrange
        var handler = Substitute.For<IHandler<POCO>>();

        handler.Handle(Arg.Any<POCO>()).Returns(Result.Success());

        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = "{}"
                }
            }
        };


        // Act
        var result = _uut.Decorate(handler, sqsEvent);

        
        // Assert
        result.BatchItemFailures.Should().BeEmpty();
    }

    [Fact]
    public void Decorate_ShouldAddMessageIdToFailedBatchItemsWhenHandlerReturnsFailure()
    {
        // Arrange
        var handler = Substitute.For<IHandler<POCO>>();

        handler.Handle(Arg.Any<POCO>()).Returns(Result.Failure(new Exception()));

        var messageId = Guid.NewGuid().ToString();
        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = messageId,
                    Body = "{}"
                }
            }
        };


        // Act
        var result = _uut.Decorate(handler, sqsEvent);


        // Assert
        result.BatchItemFailures.Single().ItemIdentifier.Should().Be(messageId);
    }

    [Fact]
    public void Decorate_ShouldLogErrorWhenExceptionIsThrownByTheHandler()
    {
        // Arrange
        var handler = Substitute.For<IHandler<POCO>>();

        handler.When(x => x.Handle(Arg.Any<POCO>()))
            .Do(x => throw new Exception("Thrown by test"));

        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = "{}"
                }
            }
        };

        // Act
        _uut.Decorate(handler, sqsEvent);
    

        // Assert
        _logger.Received(1).Error(Arg.Any<Exception>(), Arg.Any<string>());
    }

    [Fact]
    public void Decorate_ShouldLogErrorWhenHandlerReturnsFailure()
    {
        // Arrange
        var handler = Substitute.For<IHandler<POCO>>();

        var failureException = new Exception();
        handler.Handle(Arg.Any<POCO>()).Returns(Result.Failure(failureException));

        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = "{}"
                }
            }
        };


        // Act
        _uut.Decorate(handler, sqsEvent);


        // Assert
        _logger.Received(1).Error(Arg.Is(failureException), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task DecorateAsync_ShouldAddMessageIdToFailedBatchItemsWhenExceptionIsThrownByTheHandler()
    {
        // Arrange
        var handler = Substitute.For<IAsyncHandler<POCO>>();

        handler.When(x => x.HandleAsync(Arg.Any<POCO>(), Arg.Any<CancellationToken>()))
            .Do(x => throw new Exception("Thrown by test"));
            
        var messageId = Guid.NewGuid().ToString();

        var sqsEvent = new SQSEvent 
        {
            Records = new List<SQSEvent.SQSMessage> 
            {
                new() {
                    MessageId = messageId,
                    Body = "{}"
                }
            }
        };


        // Act
        var result = await _uut.DecorateAsync(handler, sqsEvent, DeterministicCancellationToken);


        // Assert
        result.BatchItemFailures.Single().ItemIdentifier.Should().Be(messageId);  
    }

    [Fact]
    public async Task DecorateAsync_ShouldNotAddMessageIdFailureBatchItemsWhenNoExceptionIsThrownByTheHandler()
    {
        // Arrange
        var handler = Substitute.For<IAsyncHandler<POCO>>();

        handler.HandleAsync(Arg.Any<POCO>(), Arg.Any<CancellationToken>()).Returns(Result.Success());

        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = "{}"
                }
            }
        };


        // Act
        var result = await _uut.DecorateAsync(handler, sqsEvent, DeterministicCancellationToken);


        // Assert
        result.BatchItemFailures.Should().BeEmpty();
    }

    [Fact]
    public async Task DecorateAsync_ShouldAddMessageIdToFailedBatchItemsWhenHandlerReturnsFailure()
    {
        // Arrange
        var handler = Substitute.For<IAsyncHandler<POCO>>();

        handler.HandleAsync(Arg.Any<POCO>(), Arg.Any<CancellationToken>()).Returns(Result.Failure(new Exception()));

        var messageId = Guid.NewGuid().ToString();
        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = messageId,
                    Body = "{}"
                }
            }
        };


        // Act
        var result = await _uut.DecorateAsync(handler, sqsEvent, DeterministicCancellationToken);


        // Assert
        result.BatchItemFailures.Single().ItemIdentifier.Should().Be(messageId);
    }

    [Fact]
    public async Task DecorateAsync_ShouldLogErrorWhenExceptionIsThrownByTheHandler()
    {
        // Arrange
        var handler = Substitute.For<IAsyncHandler<POCO>>();

        handler.When(x => x.HandleAsync(Arg.Any<POCO>(), Arg.Any<CancellationToken>()))
            .Do(x => throw new Exception("Thrown by test"));

        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = "{}"
                }
            }
        };


        // Act
        await _uut.DecorateAsync(handler, sqsEvent, DeterministicCancellationToken);
    

        // Assert
        _logger.Received(1).Error(Arg.Any<Exception>(), Arg.Any<string>());
    }

    [Fact]
    public async Task DecorateAsync_ShouldLogErrorWhenHandlerReturnsFailure()
    {
        // Arrange
        var handler = Substitute.For<IAsyncHandler<POCO>>();

        var failureException = new Exception();
        handler.HandleAsync(Arg.Any<POCO>(), Arg.Any<CancellationToken>()).Returns(Result.Failure(failureException));

        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new() 
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = "{}"
                }
            }
        };


        // Act
        await _uut.DecorateAsync(handler, sqsEvent, DeterministicCancellationToken);


        // Assert
        _logger.Received(1).Error(Arg.Is(failureException), Arg.Any<string>(), Arg.Any<string>());
    }



    public class POCO {  }
}
