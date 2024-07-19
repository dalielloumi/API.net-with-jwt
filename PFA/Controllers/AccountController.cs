using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PFA.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PFA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser(RegisterModel newUserVM)
        {
            if (ModelState.IsValid)
            {
                IdentityUser appUser = new IdentityUser
                {
                    UserName = newUserVM.Email,
                    Email = newUserVM.Email,
                    PhoneNumber = newUserVM.phoneNumber
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, newUserVM.Password);
                if (result.Succeeded)
                {
                    return Ok("success");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Erreur", error.Description);
                        _logger.LogError("User creation error: {Error}", error.Description);
                    }
                }
            }
            else
            {
                _logger.LogError("Model state is invalid: {ModelState}", ModelState);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByNameAsync(model.username);
                if (user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var secretKey = _configuration["JWT:SecretKey"];
                        if (string.IsNullOrEmpty(secretKey))
                        {
                            _logger.LogError("JWT Secret Key is missing in configuration.");
                            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please check the server logs.");
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            issuer: _configuration["JWT:Issuer"],
                            audience: _configuration["JWT:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: creds
                        );

                        var response = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            username = model.username,
                            roles
                        };
                        return Ok(response);
                    }
                    else
                    {
                        ModelState.AddModelError("Erreur", "Utilisateur n'est pas autorisé à se connecter");
                        return Unauthorized(ModelState);
                    }
                }
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }

    }
}
