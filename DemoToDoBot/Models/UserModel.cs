namespace DemoToDoBot.Models;

public class UserModel
{
    public long Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public long PhoneNumber { get; set; }

    public long ChatId { get; set; }

    public ICollection<TaskModel> Tasks { get; set; }
}
