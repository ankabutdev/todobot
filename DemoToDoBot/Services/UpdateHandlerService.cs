using DemoToDoBot.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace DemoToDoBot.Services;

public class UpdateHandlerService : IUpdateHandler
{
    private readonly IServiceScopeFactory _scopeFactory;
    private TaskModelService _taskService;

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

        var chatId = update.Message.Chat.Id;

        if (update.Message.Text == "/dailytasks")
        {
            var dailyTasks = await _taskService.GetDailyTasksAsync();
            await DisplayTasks(botClient, chatId, "Daily Tasks", dailyTasks, cancellationToken);
        }
        else if (update.Message.Text == "/archivedtasks")
        {
            var archivedTasks = await _taskService.GetArchivedTasksAsync();
            await DisplayTasks(botClient, chatId, "Archived Tasks", archivedTasks, cancellationToken);
        }
    }

    private async Task DisplayTasks(ITelegramBotClient botClient, long chatId, string title, IEnumerable<TaskModel> tasks, CancellationToken token)
    {
        var message = $"{title}:\n";
        message += tasks.Any() ? string.Join("\n", tasks.Select(t => $"- {t.Description}")) : "No tasks.";

        await botClient.SendTextMessageAsync(chatId, message, cancellationToken: token);
    }


}
