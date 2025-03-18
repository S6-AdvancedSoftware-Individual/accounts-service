using Domain.Enums;

namespace Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Biography { get; set; }
    public required AccountRole Role { get; set; } = AccountRole.User;
    public DateTime CreationAt { get; set; }
    public DateTime LastUpdatedAt { get; set;}
}
