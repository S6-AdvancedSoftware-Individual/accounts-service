using Application.Features.Accounts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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

            if (result == Guid.Empty)
            {
                Log.Information("Account creation failed");
                return BadRequest("Account creation failed");
            }

            Log.Information("Account created with ID: {Id}", result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> Get(Guid id)
        {
            var result = await _mediator.Send(new GetAccount.Query(id));

            if (result == null)
            {
                Log.Information("No account associated with ID '{Id}' found", result);
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
                Log.Information("No name or account associated with ID '{Id}' found", result);
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPatch("{id}/name")]
        public async Task<ActionResult<string>> UpdateUsername(Guid id, [FromBody] string newUsername)
        {
            var result = await _mediator.Send(new UpdateUsername.Command(id, newUsername));

            if (result == null)
            {
                Log.Information("No account associated with ID '{Id}' found", id);
                return NotFound();
            }

            return Ok(result);
        }
    }
}
