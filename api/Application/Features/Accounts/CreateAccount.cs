using Domain.Entities;
using Domain.Enums;
using MediatR;
using PostService.Application.Common.Interfaces;

namespace Application.Features.Accounts
{
    public class CreateAccount
    {
        public record Command(string Username, string Email, string Biography, AccountRole Role) : IRequest<Guid>;

        public class Handler(IAccountDbContext context) : IRequestHandler<Command, Guid>
        {
            private readonly IAccountDbContext _context = context;

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var post = new Account
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email,
                    Biography = request.Biography,
                    Role = request.Role,
                    CreationAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                };

                _context.Accounts.Add(post);
                await _context.SaveChangesAsync(cancellationToken);

                return post.Id;
            }
        }
    }
}
