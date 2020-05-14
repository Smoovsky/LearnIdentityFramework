using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LearnIdentityFramework
{
    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }

        public const string SecurityLevel = "SecurityLevel";

        public const string Rank = "Rank";
    }

    public class DynamicPolicyFactory
    {
        public static AuthorizationPolicy Create(string name)
        {
            var type = name.Split(".").First();
            var val = name.Split(".").Last();

            switch (type)
            {
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim(DynamicPolicies.Rank, val)
                        .Build();
                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                        .AddRequirements(new SecurityLevelRequirement(int.Parse(val)))
                        .Build();
                default:
                    return null;
            }
        }
    }

    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SecurityLevelRequirement requirement)
        {
            var claim = context.User.Claims
            .SingleOrDefault(c => c.Type == DynamicPolicies.SecurityLevel);

            if (claim == null)
            {
                return Task.CompletedTask;
            }

            var lv = int.Parse(claim.Value);

            if (lv >= requirement.Level)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public int Level { get; set; }

        public SecurityLevelRequirement(int lv)
        {
            Level = lv;
        }
    }

    public class CustomPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        // public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        // {
        //     throw new System.NotImplementedException();
        // }

        // public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        // {
        //     throw new System.NotImplementedException();
        // }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var dynaPolicy in DynamicPolicies.Get())
            {
                if (policyName.StartsWith(dynaPolicy))
                {
                    return Task.FromResult(DynamicPolicyFactory.Create(policyName));
                }
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}