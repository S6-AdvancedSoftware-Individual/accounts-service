using Application.Common.Interfaces;
using MediatR;
using PostService.Application.Common.Interfaces;

namespace Application.Features.Accounts;

public class UpdateUsername
{
    public record Command(Guid AuthorId, string NewUsername) : IRequest<string?>;

    public class Handler(IAccountDbContext context, IMessagePublisher publisher) : IRequestHandler<Command, string?>
    {
        private readonly IAccountDbContext _context = context;
        private readonly IMessagePublisher _publisher = publisher;

        public async Task<string?> Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts.FindAsync(new object[] { request.AuthorId }, cancellationToken);

            if (account == null)
            {
                return null;
            }

            account.Username = request.NewUsername;

            await _context.SaveChangesAsync(cancellationToken);
            await _publisher.PublishMessagesAsync([$"{request.AuthorId}:{request.NewUsername}"]);

            return account.Username;
        }
    }
}
