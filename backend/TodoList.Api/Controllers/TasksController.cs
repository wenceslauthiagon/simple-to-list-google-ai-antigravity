using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;
using TodoList.Domain.Enums;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> List(
        [FromQuery] TaskStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _taskService.ListTasksAsync(status, search, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(TaskSummaryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskSummaryResponse>> GetSummary(
        CancellationToken cancellationToken = default)
    {
        var summary = await _taskService.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetById(
        string id,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);
        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var created = await _taskService.CreateTaskAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Update(
        string id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var updated = await _taskService.UpdateTaskAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpPost("{id}/advance-status")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> AdvanceStatus(
        string id,
        CancellationToken cancellationToken = default)
    {
        var updated = await _taskService.AdvanceTaskStatusAsync(id, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("completed")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> ClearCompleted(
        CancellationToken cancellationToken = default)
    {
        var deletedCount = await _taskService.ClearCompletedTasksAsync(cancellationToken);
        return Ok(new { deletedCount });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken cancellationToken = default)
    {
        await _taskService.DeleteTaskAsync(id, cancellationToken);
        return NoContent();
    }
}
