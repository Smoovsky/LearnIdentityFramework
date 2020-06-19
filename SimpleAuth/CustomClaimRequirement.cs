using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SimpleAuth
{
    public class CustomClaimRequirement : IAuthorizationRequirement
    {
        public readonly string claimType;

        public CustomClaimRequirement(string claimType)
        {
            this.claimType = claimType;
        }
    }

    public class CustomClaimAuthHandler : AuthorizationHandler<CustomClaimRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CustomClaimRequirement requirement)
        {
            if (context.User.Claims.Any(c => c.Type == requirement.claimType))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}