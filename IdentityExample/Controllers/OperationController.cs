using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public OperationsController(
            IAuthorizationService authorizationService
        )
        {
            _authorizationService = authorizationService;
        }

        public IActionResult Attack()
        {
            var requirement = new OperationAuthorizationRequirement()
            {
                Name = SampleOperation.Attack
            };

            var authResult = _authorizationService
                .AuthorizeAsync(User, new SampleResource()
                {
                    Name = "233"
                }, requirement).Result;

            return Ok();
        }
    }

    public class SampleAuthHandler
        : AuthorizationHandler<OperationAuthorizationRequirement> // need to be added to service
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement)
        {
            var sampleResource = context.Resource;

            if (requirement.Name == SampleOperation.Attack)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement.Name == SampleOperation.Defend)
            {
                if (context.User.HasClaim("Class", "Warrior"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    public static class SampleOperation
    {
        public static string Attack = "attack";
        public static string Defend = "defend";
        public static string Cast = "cast";
    }

    public class SampleResource
    {
        public string Name { get; set; }
    }
}