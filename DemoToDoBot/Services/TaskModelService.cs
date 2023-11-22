using DemoToDoBot.Data;
using DemoToDoBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoToDoBot.Services;

public class TaskModelService
{
    private readonly BotDbContext _dbContext;

    public TaskModelService(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskModel> CreateTaskAsync(TaskModel task)
    {
        var taskEntity = new TaskModel
        {
            Description = task.Description,
            DueDate = task.DueDate,
            IsArchived = task.IsArchived
        };

        await _dbContext.Tasks.AddAsync(taskEntity);
        await _dbContext.SaveChangesAsync();

        task.Id = taskEntity.Id; // Assign the generated ID back to the TaskModel

        return task;
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId);
        if (taskEntity != null)
        {
            _dbContext.Tasks.Remove(taskEntity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TaskModel>> GetArchivedTasksAsync()
    {
        var archivedTaskEntities = await _dbContext.Tasks.Where(t => t.IsArchived).ToListAsync();
        return archivedTaskEntities.Select(MapToTaskModel);
    }

    public async Task<IEnumerable<TaskModel>> GetDailyTasksAsync()
    {
        var today = DateTime.Today;
        var dailyTaskEntities = await _dbContext.Tasks
            .Where(t => !t.IsArchived && t.DueDate.Date == today)
            .ToListAsync();

        return dailyTaskEntities.Select(MapToTaskModel);
    }

    public async Task<TaskModel> GetTaskByIdAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId);
        return taskEntity != null ? MapToTaskModel(taskEntity) : null;
    }

    public async Task UpdateTaskAsync(TaskModel task)
    {
        var existingTaskEntity = await _dbContext.Tasks.FindAsync(task.Id);
        if (existingTaskEntity != null)
        {
            existingTaskEntity.Description = task.Description;
            existingTaskEntity.DueDate = task.DueDate;
            existingTaskEntity.IsArchived = task.IsArchived;

            await _dbContext.SaveChangesAsync();
        }
    }

    private static TaskModel MapToTaskModel(TaskModel taskEntity)
    {
        return new TaskModel
        {
            Id = taskEntity.Id,
            Description = taskEntity.Description,
            DueDate = taskEntity.DueDate,
            IsArchived = taskEntity.IsArchived
        };
    }
}
