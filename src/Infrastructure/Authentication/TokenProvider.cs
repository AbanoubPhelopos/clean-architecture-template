using System.Security.Claims;
using System.Text;
using Application.Abstractions.Authentication;
using Application.Configuration;
using Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider(IOptions<JwtSettings> jwtSettings) : ITokenProvider
{
    public string Create(User user)
    {
        JwtSettings settings = jwtSettings.Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(settings.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = settings.Issuer,
            Audience = settings.Audience
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}
