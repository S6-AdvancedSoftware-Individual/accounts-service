namespace Application.Common.Interfaces;

public interface IMessagePublisher
{
    Task PublishMessagesAsync(IEnumerable<string> messages);
}
