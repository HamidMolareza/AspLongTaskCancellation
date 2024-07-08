using AspLongTaskCancellation.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AspLongTaskCancellation.Controllers;

[ApiController]
[Route("[controller]")]
public class LongRunningController(ILogger<LongRunningController> logger) : ControllerBase {
    [HttpGet("WithoutCancellationToken")]
    public async Task<ActionResult<ResponseMessageDto>> WithoutCancellationToken(List<int>? delays = null) {
        delays ??= [4000, 8000, 15000];

        logger.LogInformation("{Id} - Started", HttpContext.TraceIdentifier);
        for (var i = 0; i < delays.Count; i++) 
            await DelayAsync($"Task {i + 1}", delays[i]);

        return Ok(new ResponseMessageDto("Request completed successfully."));
    }

    private async Task DelayAsync(string taskName, int delayInMilliseconds) {
        await Task.Delay(delayInMilliseconds);
        logger.LogInformation("{Id} - {TaskName} was done", HttpContext.TraceIdentifier, taskName);
    }
}