using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Features.Tasks.Queries;

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
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery(id));
        if (task == null) return NotFound();
        return Ok(task);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTasks([FromQuery] TaskStatus? status)
    {
        var query = new GetAllTasksQuery(status);
        var tasks = await _mediator.Send(query);
        return Ok(tasks);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        var success = await _mediator.Send(command);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }


}