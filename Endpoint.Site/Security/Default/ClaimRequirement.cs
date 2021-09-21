using Microsoft.AspNetCore.Authorization;

namespace Endpoint.Site.Security.Default
{
    public class ClaimRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public ClaimRequirement(string claimType, string claimValue)
        {
            ClaimType = claimType;
            this.ClaimValue = claimValue;
        }
    }
}
