using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SimpleAuth.ClaimTransform
{
    public class ClaimTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if(!principal.Claims.Any(c => c.Type == "Sample"))
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("Sample", "Val"));
            }

            return Task.FromResult(principal);
        }
    }
}