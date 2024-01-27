using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyTemplate.CrossCutting.Extensions;
using Serilog;
using Serilog.Events;
using Xunit;

namespace MyTemplate.UnitTests.CrossCutting.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddLogger_ShouldAddLoggerWithMinimumLogLevelOfInformationToServiceCollectionWhenNoLogLevelEventIsProvided()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();


        // Act
        serviceCollection.AddLogger();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger>();


        // Assert
        logger.IsEnabled(LogEventLevel.Information).Should().BeTrue();
        logger.IsEnabled(LogEventLevel.Debug).Should().BeFalse();
        logger.IsEnabled(LogEventLevel.Verbose).Should().BeFalse();
    }

    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Debug)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Error)]
    [InlineData(LogEventLevel.Fatal)]
    public void AddLogger_ShouldAddLoggerWithMinimumLogLevelAsProvidedByTheUser(LogEventLevel logLevel)
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        

        // Act
        serviceCollection.AddLogger(logLevel);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger>();


        // Assert
        logger.IsEnabled(logLevel).Should().BeTrue();
    }

    [Fact]
    public void AddLogger_ShouldNotReplaceLoggerWhenLoggerIsAlreadyRegistered()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Fatal()
            .CreateLogger();

        serviceCollection.AddSingleton<ILogger>(logger);
    

        // Act
        serviceCollection.AddLogger();

        var serviceProvider = serviceCollection.BuildServiceProvider();


        // Assert
        serviceProvider.GetRequiredService<ILogger>().Should().Be(logger);
    }
}
