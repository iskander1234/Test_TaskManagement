using AutoMapper;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Commands;

namespace TaskManagement.Application.MappingProfiles;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<CreateTaskCommand, TaskEntity>();
    }
}