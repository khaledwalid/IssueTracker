using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IssueTracker.Common.Base
{
    public class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        protected IMediator Mediator => _mediator ?? HttpContext.RequestServices.GetService<IMediator>();

        public BaseController(ILogger logger)
        {
        }

        protected virtual IActionResult Success(string message, object response,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var result = new BodyResponse
            {
                Message = message,
                Response = response
            };
            return statusCode == HttpStatusCode.OK
                ? Ok(result)
                : StatusCode((int)statusCode, result);
        }

        protected virtual IActionResult CreatedWithSuccess(string message, object response)
        {
            return Success(message, response, HttpStatusCode.Created);
        }

        protected virtual IActionResult Failure(HttpStatusCode statusCode, string message, object response = null)
        {
            return StatusCode((int)statusCode, new BodyResponse
            {
                Message = message,
                Response = response
            });
        }
    }
}