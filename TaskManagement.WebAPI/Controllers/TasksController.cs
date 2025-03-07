using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Tasks.Commands;

namespace TaskManagement.WebAPI.Controllers;

[Route("api/tasks")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
    {
        var taskId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTaskById), new { id = taskId }, new { id = taskId });
    }

    [HttpGet("{id}")]
    public IActionResult GetTaskById(Guid id)
    {
        return Ok($"Stub for getting task {id}");
    }
}