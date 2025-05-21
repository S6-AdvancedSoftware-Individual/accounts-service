using Domain.Entities;
using MediatR;
using PostService.Application.Common.Interfaces;

namespace Application.Features.Accounts;

public class GetAccount
{
    public record Query(Guid Id) : IRequest<Account>;

    public class Handler(IAccountDbContext context) : IRequestHandler<Query, Account>
    {
        private readonly IAccountDbContext _context = context;

        public async Task<Account> Handle(Query request, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts.FindAsync(new object[] { request.Id }, cancellationToken);

            return account;
        }
    }
}