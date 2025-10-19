using Banque.API.Data;
using Banque.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Banque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BanqueContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(BanqueContext context, IConfiguration config, ILogger<AuthController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        // ✅ Connexion d'un utilisateur
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Vérifie si l’utilisateur existe
            var user = await _context.Clients.FirstOrDefaultAsync(c => c.Email == request.Email);
            if (user == null)
                return Unauthorized("Identifiants invalides.");

            // Vérifie le mot de passe
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Identifiants invalides.");

            // 🔹 Récupération et validation de la config JWT
            var jwt = _config.GetSection("Jwt");

            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var keyString = jwt["Key"];
            var expireMinutes = jwt["ExpireMinutes"];

            if (string.IsNullOrWhiteSpace(keyString))
                return StatusCode(500, "Clé JWT manquante dans appsettings.json");

            var keyBytes = Encoding.UTF8.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 🔹 Claims utilisateur
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Name, user.Nom),
                new(ClaimTypes.Role, "Client")
            };

            // 🔹 Génération du token
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(expireMinutes ?? "60"));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 🔍 Log de débogage (affiché dans la console)
            _logger.LogInformation("✅ JWT généré pour {Email}", user.Email);
            _logger.LogInformation("🔑 Clé utilisée : {Key}", keyString);
            _logger.LogInformation("🧾 Issuer: {Issuer}, Audience: {Audience}", issuer, audience);
            _logger.LogInformation("⏰ Expiration : {Expire}", expires);

            return Ok(new LoginResponse(tokenString, expires));
        }

        // ✅ Enregistrement d'un nouveau client
        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginRequest request)
        {
            if (await _context.Clients.AnyAsync(c => c.Email == request.Email))
                return BadRequest("Email déjà utilisé.");

            var client = new Models.Client
            {
                Nom = request.Email.Split('@')[0],
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok("Compte client créé avec succès.");
        }
    }
}
