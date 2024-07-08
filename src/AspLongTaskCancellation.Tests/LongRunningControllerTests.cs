using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using AspLongTaskCancellation.Controllers;
using AspLongTaskCancellation.Dto;
using Microsoft.AspNetCore.Http;

namespace AspLongTaskCancellation.Tests;

public class LongRunningControllerTests {
    private readonly Mock<ILogger<LongRunningController>> _loggerMock;
    private readonly LongRunningController _controller;

    public LongRunningControllerTests() {
        _loggerMock = new Mock<ILogger<LongRunningController>>();
        _controller = new LongRunningController(_loggerMock.Object);
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task WithoutCancellationToken_ReturnsOkResult() {
        // Act
        var response = await _controller.WithoutCancellationToken([]);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        var value = Assert.IsType<ResponseMessageDto>(okResult.Value);
        Assert.Equal("Request completed successfully.", value.Message);
    }

    [Fact]
    public async Task WithoutCancellationToken_LogsMessages() {
        // Act
        var delays = new List<int> { 1, 1, 1 };
        await _controller.WithoutCancellationToken([1, 1, 1]);
        var totalRepeat = delays.Count;

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Started")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

        for (var i = 0; i < totalRepeat; i++) {
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Task {i + 1} was done")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }
    }

    [Fact]
    public async Task WithCancellationToken_ReturnsOkResult() {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        var response = await _controller.WithCancellationToken(cancellationToken, 2, 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        var value = Assert.IsType<ResponseMessageDto>(okResult.Value);
        Assert.Equal("Long-running request completed successfully.", value.Message);
    }

    [Fact]
    public async Task WithCancellationToken_ThrowsWhenCancelled() {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        await cancellationTokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _controller.WithCancellationToken(cancellationToken));
    }

    [Fact]
    public async Task WithCancellationToken_LogsMessages() {
        // Arrange
        var cancellationToken = new CancellationToken();
        const int totalRepeat = 2;

        // Act
        await _controller.WithCancellationToken(cancellationToken, totalRepeat, 1);

        // Assert
        for (var i = 0; i < totalRepeat; i++) {
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"{i + 1}/{totalRepeat}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }
    }
}