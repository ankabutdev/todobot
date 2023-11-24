using DemoToDoBot.Models;

namespace DemoToDoBot.Interfaces;

public interface ITaskModelService
{
    Task<IEnumerable<TaskModel>> GetDailyTasksAsync();

    Task<IEnumerable<TaskModel>> GetArchivedTasksAsync();

    Task<TaskModel> GetTaskByIdAsync(int taskId);

    Task<TaskModel> CreateTaskAsync(TaskModel task);

    Task UpdateTaskAsync(TaskModel task);

    Task DeleteTaskAsync(int taskId);

    Task<IEnumerable<TaskModel>> SendTasksAllUsersAsync();

    Task<IEnumerable<TaskModel>> GetAllTasksAsync();
}
