using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Velora.Application.Common.Interfaces;
using Velora.Core.Entities;

namespace Velora.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public IdentityService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<(bool Succeeded, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.Succeeded, user.Id);
    }

    public async Task<(bool Succeeded, string Token)> LoginAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return (false, "Geçersiz kullanıcı adı veya şifre.");

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result) return (false, "Geçersiz kullanıcı adı veya şifre.");

        var token = GenerateJwtToken(user);
        return (true, token);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> CheckPasswordAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }
}
