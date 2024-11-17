namespace dotnet_MultiJwtAuth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

internal sealed class MultiJwtClaimsTransformation(IConfiguration configuration) :IClaimsTransformation
{
    private const string AuthSource ="auth_source";
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
       if(principal.HasClaim(c=>c.Type==AuthSource))
       {
        return Task.FromResult(principal);
       }

       var cliamsIdentity = new ClaimsIdentity();
       string? issuer = principal
                .Identities
                .Select(i=>i.FindFirst(JwtRegisteredClaimNames.Iss)?.Value)
                .FirstOrDefault();

        if(issuer == configuration["Authentication:Keycloak:ValidIssuer"])
        {
            cliamsIdentity.AddClaim(new Claim(AuthSource, CustomAuthSchemes.Keyccloak));
        }
        else if(issuer == configuration["Authentication:Supabase:ValidIssuer"])
        {
            cliamsIdentity.AddClaim(new Claim(AuthSource, CustomAuthSchemes.Supabase));
        }
        principal.AddIdentity(cliamsIdentity);

        return Task.FromResult(principal);
    }
}

