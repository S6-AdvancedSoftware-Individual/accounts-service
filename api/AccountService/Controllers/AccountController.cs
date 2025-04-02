using Application.Features.Accounts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/accounts")]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateAccount.Command command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> Get(Guid id)
        {
            var result = await _mediator.Send(new GetAccount.Query(id));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{id}/name")]
        public async Task<ActionResult<string>> GetAccountName(Guid id)
        {
            var result = await _mediator.Send(new GetAccountName.Query(id));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
