using DemoToDoBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;

public class BotBackgroundService : BackgroundService
{
    private readonly TelegramBotClient _botClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BotBackgroundService> _logger;
    private readonly IUpdateHandler _handler;
    private UserModelService _userService;
    private TaskModelService _taskService;

    public BotBackgroundService(TelegramBotClient botClient,
        IServiceScopeFactory scopeFactory,
        ILogger<BotBackgroundService> logger,
        IUpdateHandler handler)
    {
        _botClient = botClient;
        _scopeFactory = scopeFactory;
        _handler = handler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        _userService = scope.ServiceProvider.GetRequiredService<UserModelService>();
        _taskService = scope.ServiceProvider.GetRequiredService<TaskModelService>();

        var bot = await _botClient.GetMeAsync(stoppingToken);

        _logger.LogInformation($"Bot started successfully: {bot.Username}");

        _botClient.StartReceiving(
            _handler.HandleUpdateAsync,
            _handler.HandlePollingErrorAsync,
            new ReceiverOptions()
            {
                ThrowPendingUpdates = true
            }, stoppingToken);

        await Task.Delay(1000);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                if (now.Hour == 17 && now.Minute == 38 && now.Second == 0)
                {
                    var tasks = await _taskService.SendTasksAllUsersAsync();

                    foreach (var task in tasks)
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: task.UserId,
                            text: task.Description,
                            cancellationToken: stoppingToken);
                        task.DueDate = now;
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service");
            }
        }
    }
}
