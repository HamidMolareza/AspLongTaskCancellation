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
        await _controller.WithoutCancellationToken([1, 1, 1]);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Started")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Task 1 was done")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Task 2 was done")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Task 3 was done")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}