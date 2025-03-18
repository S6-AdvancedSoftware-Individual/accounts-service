using Application.Features.Accounts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

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
    }
}
