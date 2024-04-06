using Globomantics.Domain;
using Microsoft.EntityFrameworkCore;

namespace Globomantics.Infrastructure.Data.Repositories;

public class TodoTaskRepository : TodoRepository<TodoTask>
{
    public TodoTaskRepository(GlobomanticsDbContext context) : base(context)
    {
    }

    public async override Task AddAsync(TodoTask todoTask)
    {
        var todoTaskToAdd = DomainToDataMapping.MapTodoFromDomain<TodoTask, Data.Models.TodoTask> (todoTask);

        await Context.AddAsync(todoTaskToAdd);
    }

    public async override Task<TodoTask> GetAsync(Guid id)
    {
        var data = await Context.TodoTasks.SingleAsync(task => task.Id == id);

        return DataToDomainMapping.MapTodoFromData<Data.Models.TodoTask, TodoTask>(data);
    }
}