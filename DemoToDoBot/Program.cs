using DemoToDoBot.Data;
using DemoToDoBot.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BotDbContext>(options =>
{
    options
        .UseSqlServer(builder
        .Configuration
        .GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<TaskModelService>();
builder.Services.AddScoped<UserModelService>();

var token = builder.Configuration["token"];
builder.Services.AddSingleton(new TelegramBotClient(token!));
builder.Services.AddSingleton<IUpdateHandler, UpdateHandlerService>();
builder.Services.AddHostedService<BotBackgroundService>();

var app = builder.Build();

app.Run();