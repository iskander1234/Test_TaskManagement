using FluentValidation;
using TaskManagement.Application.Features.Tasks.Queries;

namespace TaskManagement.Application.Validators;

public class GetAllTasksQueryValidator : AbstractValidator<GetAllTasksQuery>
{
    public GetAllTasksQueryValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status должен быть одним из доступных значений: ToDo, InProgress, Done.");
    }
}