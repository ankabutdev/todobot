using DemoToDoBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoToDoBot.Data;

public class BotDbContext : DbContext
{
    public BotDbContext(DbContextOptions<BotDbContext> options)
        : base(options) { }

    public virtual DbSet<TaskModel>? Tasks { get; set; }

    public virtual DbSet<UserModel>? Users { get; set; }
}
