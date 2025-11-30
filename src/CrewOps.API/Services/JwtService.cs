using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CrewOps.API.Models;

namespace CrewOps.API.Services;

public class JwtService
{
    // Hardcoded key for development - in production, use configuration/secrets
    private const string SecretKey = "SuperSecretKey1234567890_ThisMustBeLongEnough";
    private const int ExpirationHours = 24;

    public static string SecretKeyValue => SecretKey;

    public string GenerateToken(CrewMember member)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
            new Claim(ClaimTypes.Email, member.Email ?? ""),
            new Claim(ClaimTypes.Name, $"{member.FirstName} {member.LastName}"),
            new Claim(ClaimTypes.Role, member.Role)
        };

        var token = new JwtSecurityToken(
            issuer: "CrewOps",
            audience: "CrewOps",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(ExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
