<div align="center">
  <h1>ASP Long Task Cancellation</h1>
  <br />
  <a href="https://github.com/HamidMolareza/AspLongTaskCancellation/issues/new?assignees=&labels=bug&template=BUG_REPORT.md&title=bug%3A+">Report a Bug</a>
  ·
  <a href="https://github.com/HamidMolareza/AspLongTaskCancellation/issues/new?assignees=&labels=enhancement&template=FEATURE_REQUEST.md&title=feat%3A+">Request a Feature</a>
  .
  <a href="https://github.com/HamidMolareza/AspLongTaskCancellation/issues/new?assignees=&labels=question&template=SUPPORT_QUESTION.md&title=support%3A+">Ask a Question</a>
</div>

<div align="center">
<br />

![Build Status](https://github.com/HamidMolareza/AspLongTaskCancellation/actions/workflows/build.yaml/badge.svg?branch=main)

![GitHub](https://img.shields.io/github/license/HamidMolareza/AspLongTaskCancellation)
[![Pull Requests welcome](https://img.shields.io/badge/PRs-welcome-ff69b4.svg?style=flat-square)](https://github.com/HamidMolareza/AspLongTaskCancellation/issues?q=is%3Aissue+is%3Aopen+label%3A%22help+wanted%22)

[![code with love by HamidMolareza](https://img.shields.io/badge/%3C%2F%3E%20with%20%E2%99%A5%20by-HamidMolareza-ff1414.svg?style=flat-square)](https://github.com/HamidMolareza)

</div>

## About

### Overview

The **AspLongTaskCancellation** project is an ASP.NET application designed to demonstrate the importance and implementation of `CancellationToken` for managing long-running tasks. This project highlights the performance and resource management benefits that come from properly handling user request cancellations.

### Purpose and Problem

**Problem:** In typical ASP.NET applications, long-running tasks continue to execute even if a user cancels their request. This can lead to wasted resources and reduced performance, especially under heavy load or when dealing with numerous simultaneous requests.

**Purpose:** The primary goal of this project is to illustrate how to use `CancellationToken` in ASP.NET to efficiently handle user cancellations and improve application performance by stopping tasks that no longer need to be processed.

### Benefits

- **Improved Performance:** By canceling long-running tasks that are no longer needed, the application can run more efficiently, reducing server load and improving response times.
- **Resource Management:** Properly handling task cancellations helps in freeing up resources that would otherwise be wasted on unnecessary operations.
- **User Experience:** Users benefit from faster and more responsive applications when server resources are managed effectively.
- **Scalability:** Applications that handle resource management well are better equipped to scale and handle increased traffic.

### Built With

- .NET 8

## Getting Started

### Prerequisites

- .NET SDK
- Docker (optional)

### Running the Application

1. Clone the repository:

2. Restore dependencies:
   ```sh
   dotnet restore
   ```

3. Run the application:
   ```sh
   dotnet run
   ```

### Running with Docker

1. Build the Docker image:
   ```sh
   docker build -t asp-long-task-cancellation .
   ```

2. Run the Docker container:
   ```sh
   docker run --rm -p 8080:8080 asp-long-task-cancellation
   ```

3. The API will be available at `http://localhost:8080`.

### Endpoints

- `/LongRunning/WithoutCancellationToken`
- `/LongRunning/WithCancellationToken`

### Without `CancellationToken`

The `/LongRunning/WithoutCancellationToken` endpoint simulates a long-running task without using `CancellationToken`. The task will continue to run even if the request is cancelled.

```csharp
[HttpGet("WithoutCancellationToken")]
public async Task<ActionResult<ResponseMessageDto>> WithoutCancellationToken(List<int>? delays = null) {
    delays ??= [4000, 8000, 15000];

    logger.LogInformation("{Id} - Started", HttpContext.TraceIdentifier);
    for (var i = 0; i < delays.Count; i++)
        await DelayAsync($"Task {i + 1}", delays[i]);

    return Ok(new ResponseMessageDto("Request completed successfully."));
}
```

### With `CancellationToken`

The `/LongRunning/WithCancellationToken` endpoint uses `CancellationToken` to cancel the task if the request is cancelled.

```csharp
[HttpGet("WithCancellationToken")]
public async Task<ActionResult<ResponseMessageDto>> WithCancellationToken(CancellationToken cancellationToken,
    int? total = null, int? delay = null) {
    total ??= 10;
    delay ??= 1000;

    for (var i = 0; i < total; i++) {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay((int)delay, cancellationToken);
        logger.LogInformation("{Id}: {Index}/{Total}", HttpContext.TraceIdentifier, i + 1, total);
    }

    return Ok(new ResponseMessageDto("Long-running request completed successfully."));
}
```

## Conclusion

By using `CancellationToken`, we can improve the performance of our application by ensuring that long-running tasks are cancelled when the user cancels their request. This project demonstrates the problem and the solution with clear examples.


### Project Structure

```txt
├── AspLongTaskCancellation
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── AspLongTaskCancellation.csproj
│   ├── AspLongTaskCancellation.http
│   ├── Controllers
│   │   └── LongRunningController.cs
│   ├── Dto
│   │   └── ResponseMessageDto.cs
│   ├── Middlewares
│   │   └── OperationCanceledExceptionMiddleware.cs
│   ├── Program.cs
│   └── Properties
│       └── launchSettings.json
├── AspLongTaskCancellation.sln
├── AspLongTaskCancellation.Tests
│   ├── AspLongTaskCancellation.Tests.csproj
│   ├── GlobalUsings.cs
│   └── LongRunningControllerTests.cs
└── Dockerfile
```

## Support

Reach out to the maintainer at one of the following places:

- [GitHub issues](https://github.com/HamidMolareza/AspLongTaskCancellation/issues/new?assignees=&labels=question&template=SUPPORT_QUESTION.md&title=support%3A+)
- Contact options listed on [this GitHub profile](https://github.com/HamidMolareza)

## FAQ

#### What is the purpose of this project?

The purpose of this project is to demonstrate how to use `CancellationToken` in ASP.NET to efficiently handle long-running tasks and improve application performance by stopping tasks that no longer need to be processed when a user cancels their request.

#### What problem does this project solve?

This project addresses the issue of long-running tasks continuing to execute even after a user cancels their request. This can lead to wasted resources and reduced performance. By implementing `CancellationToken`, the project shows how to cancel these tasks, thereby improving resource management and application efficiency.

## Related

- [Should I always add CancellationToken to my controller actions?](https://stackoverflow.com/questions/50329618/should-i-always-add-cancellationtoken-to-my-controller-actions)

## License

This project is licensed under the **GPLv3**.

See [LICENSE](LICENSE) for more information.
