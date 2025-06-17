using Domain.Entities;
using Domain.Enums;
using MediatR;
using PostService.Application.Common.Interfaces;

namespace Application.Features.Accounts
{
    public class CreateAccount
    {
        public record Command(string Username, AccountRole? Role, string? auth0UserId) : IRequest<Guid>;

        public class Handler(IAccountDbContext context) : IRequestHandler<Command, Guid>
        {
            private readonly IAccountDbContext _context = context;

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Role = request.Role ?? AccountRole.User,
                    CreationAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    Auth0UserId = request.auth0UserId ?? ""
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync(cancellationToken);

                return account.Id;
            }
        }
    }
}
