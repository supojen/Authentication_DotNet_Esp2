using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace esp2.Helper
{
    public class SelfDefineUserClaimsPrincipalFactory : 
        UserClaimsPrincipalFactory<IdentityUser>
    {
        public SelfDefineUserClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {}


        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var claims = await base.GenerateClaimsAsync(user);
            return claims;
        }
    }
}
