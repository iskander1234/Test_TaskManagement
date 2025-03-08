using FluentValidation;
using TaskManagement.Application.Features.Tasks.Queries;

namespace TaskManagement.Application.Validators;

public class GetAllTasksQueryValidator : AbstractValidator<GetAllTasksQuery>
{
    public GetAllTasksQueryValidator()
    {
        RuleFor(x => x.Status)
            .Must(value => value == null || Enum.IsDefined(typeof(TaskStatus), value.Value))
            .WithMessage("Status должен быть одним из доступных значений: 0 (ToDo), 1 (InProgress), 2 (Done).");
    }
}