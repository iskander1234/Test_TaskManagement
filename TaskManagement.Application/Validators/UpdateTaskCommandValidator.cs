using FluentValidation;
using TaskManagement.Application.Features.Tasks.Commands;

namespace TaskManagement.Application.Validators;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

        RuleFor(x => x.Status)
            .Must(status => Enum.IsDefined(typeof(TaskStatus), status))
            .WithMessage("Invalid task status.");
    }
}