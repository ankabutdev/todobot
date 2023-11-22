using System.ComponentModel.DataAnnotations.Schema;

namespace DemoToDoBot.Models;

public class TaskModel
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public bool IsArchived { get; set; }

    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel Users { get; set; }
}
