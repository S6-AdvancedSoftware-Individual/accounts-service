using Domain.Enums;

namespace Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string? Auth0UserId { get; set; } = null;
    public required string Username { get; set; }
    public required AccountRole Role { get; set; } = AccountRole.User;
    public DateTime CreationAt { get; set; }
    public DateTime LastUpdatedAt { get; set;}
}
