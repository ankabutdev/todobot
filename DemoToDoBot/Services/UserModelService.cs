using DemoToDoBot.Data;
using DemoToDoBot.Interfaces;
using DemoToDoBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoToDoBot.Services;

public class UserModelService : IUserModelService
{
    private readonly BotDbContext _dbContext;

    public UserModelService(BotDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new NullReferenceException(nameof(dbContext));
    }

    public async Task<UserModel> CreateTaskAsync(UserModel user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task DeleteTaskAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
       => await _dbContext.Users.ToListAsync();

    public async Task<IEnumerable<UserModel>> GetArchivedTasksAsync()
    {
        return await _dbContext.Users
            .Include(u => u.Tasks)
            .Where(u => u.Tasks
            .Any(t => t.IsArchived))
            .ToListAsync();
    }

    public async Task<IEnumerable<UserModel>> GetDailyTasksAsync()
    {
        var today = DateTime.Today;
        return await _dbContext.Users
            .Include(u => u.Tasks)
            .Where(u => u.Tasks
            .Any(t => t.DueDate.Date == today))
            .ToListAsync();
    }

    public async Task<UserModel> GetTaskByIdAsync(int userId)
    {
        return await _dbContext
            .Users
            .Include(u => u.Tasks)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task UpdateTaskAsync(UserModel user)
    {
        _dbContext.Entry(user).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }
}