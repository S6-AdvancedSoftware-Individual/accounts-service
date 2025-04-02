using Domain.Entities;
using MediatR;
using PostService.Application.Common.Interfaces;

namespace Application.Features.Accounts;

public class GetAccountName
{
    public record Query(Guid Id) : IRequest<string?>;

    public class Handler(IAccountDbContext context) : IRequestHandler<Query, string?>
    {
        private readonly IAccountDbContext _context = context;

        public async Task<string?> Handle(Query request, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts.FindAsync(new object[] { request.Id }, cancellationToken);

            if(account == null)
            {
                return null;
            }

            return account.Username;
        }
    }
}