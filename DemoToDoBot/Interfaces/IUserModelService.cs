using DemoToDoBot.Models;

namespace DemoToDoBot.Interfaces;

public interface IUserModelService
{
    Task<IEnumerable<UserModel>> GetDailyTasksAsync();

    Task<IEnumerable<UserModel>> GetArchivedTasksAsync();

    Task<UserModel> GetTaskByIdAsync(int userId);

    Task<UserModel> CreateTaskAsync(UserModel user);

    Task UpdateTaskAsync(UserModel user);

    Task DeleteTaskAsync(int userId);

    Task<IEnumerable<UserModel>> GetAllUsersAsync();
}