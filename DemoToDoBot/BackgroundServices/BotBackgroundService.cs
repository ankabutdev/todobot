using Telegram.Bot;
using Telegram.Bot.Polling;

namespace DemoToDoBot.BackgroundServices;

public class BotBackgroundService : BackgroundService
{
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<BotBackgroundService> _logger;
    private readonly IUpdateHandler _handler;

    public BotBackgroundService(TelegramBotClient botClient,
        ILogger<BotBackgroundService> logger,
        IUpdateHandler handler)
    {
        _botClient = botClient;
        _handler = handler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bot = await _botClient.GetMeAsync(stoppingToken);

        _logger.LogInformation("Bot started successfully: {bot.Username}", bot.Username);

        _botClient.StartReceiving(
            _handler.HandleUpdateAsync,
            _handler.HandlePollingErrorAsync,
            new ReceiverOptions()
            {
                ThrowPendingUpdates = true
            }, stoppingToken);
    }
}