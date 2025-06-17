using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PostService.Application.Common.Interfaces;

namespace Application.Features.Accounts;

public class GetAccountByAuth0UserId
{
    public record Query(string Auth0UserId) : IRequest<Account?>;
    public class Handler(IAccountDbContext context) : IRequestHandler<Query, Account?>
    {
        private readonly IAccountDbContext _context = context;
        public async Task<Account?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Auth0UserId == request.Auth0UserId, cancellationToken);
        }
    }
}
