using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniAts.Controllers;

public record LoginDto(string Email, string Senha);
public record TokenDto(string Token, DateTime Expiracao);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public ActionResult<TokenDto> Login(LoginDto dto)
    {
        // provisório: em produção isso valida contra o banco com hash
        if (dto.Email != "admin@miniats.com" || dto.Senha != "senha123")
            return Unauthorized();

        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Chave"]!));

        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "1"),
            new Claim(JwtRegisteredClaimNames.Email, dto.Email),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiracao = DateTime.UtcNow.AddMinutes(
            int.Parse(_config["Jwt:ExpiracaoMinutos"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Emissor"],
            audience: _config["Jwt:Audiencia"],
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais);

        return Ok(new TokenDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiracao));
    }
}