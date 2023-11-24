using DemoToDoBot.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace DemoToDoBot.Services;

public class UpdateHandlerService : IUpdateHandler
{
    private readonly IServiceScopeFactory _scopeFactory;
    private TaskModelService _taskService;
    private UserModelService _userService;

    public UpdateHandlerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        _taskService = scope.ServiceProvider.GetRequiredService<TaskModelService>();
        _userService = scope.ServiceProvider.GetRequiredService<UserModelService>();

        var chatId = update.Message.Chat.Id;

        var user = update.Message.From;

        var users = await _userService.GetAllUsersAsync();

        if (update.Message.Text == "/start")
        {
            if (users.Any(x => x.Id == user.Id))
            {
                UserModel newUser = new UserModel
                {
                    Username = user.Username,
                    ChatId = chatId
                };

                await _userService.CreateTaskAsync(newUser);

                await botClient.SendTextMessageAsync(chatId, "Registration successful! You can now use the bot.", cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "You are already registered.", cancellationToken: cancellationToken);
            }
        }
        else if (update.Message.Text == "/dailytasks")
        {
            var dailyTasks = await _taskService.GetDailyTasksAsync();
            await DisplayTasks(botClient, chatId, "Daily Tasks", dailyTasks, cancellationToken);
        }
        else if (update.Message.Text == "/archivedtasks")
        {
            var archivedTasks = await _taskService.GetArchivedTasksAsync();
            await DisplayTasks(botClient, chatId, "Archived Tasks", archivedTasks, cancellationToken);
        }
        else if (update.Message.Text == "/createtask")
        {

        }
        else if (update.Message.Text == "/")
        {

        }
    }

    private async Task DisplayTasks(ITelegramBotClient botClient, long chatId, string title, IEnumerable<TaskModel> tasks, CancellationToken token)
    {
        var message = $"{title}:\n";
        message += tasks.Any() ? string.Join("\n", tasks.Select(t => $"- {t.Description} (Due: {t.DueDate})")) : "No tasks.";

        await botClient.SendTextMessageAsync(chatId, message, cancellationToken: token);
    }
}
