namespace BattleAuth.Service.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Contracts.Domain.V1;

    public interface IJwtService
    {
        IList<Claim> BuildClaims(IList<Claim> claims, User user, DateTime now, long expiresAt);

        JwtResponse CreateToken(IList<Claim> claims, DateTime now, long expiresAt);

        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}