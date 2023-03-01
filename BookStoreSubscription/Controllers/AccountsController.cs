using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStoreSubscription.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using BookStoreSubscription.Services;

namespace BookStoreSubscription.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountsController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("register")] // api/accounts/register
        public async Task<ActionResult<AuthResponseDTO>> Register(UserCredentialDTO userCredentialDTO)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialDTO.Email,
                Email = userCredentialDTO.Email
            };
            var result = await userManager.CreateAsync(user, userCredentialDTO.Password);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentialDTO, user.Id);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(UserCredentialDTO userCredentialDTO)
        {
            var resultado = await signInManager.PasswordSignInAsync(userCredentialDTO.Email,
                userCredentialDTO.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(userCredentialDTO.Email);
                return await BuildToken(userCredentialDTO, user.Id);
            }
            else
            {
                return BadRequest("Bad login");
            }
        }

        [HttpGet("refreshToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthResponseDTO>> Refresh()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var userIdClaim = HttpContext.User.Claims.Where(claim => claim.Type == "id").FirstOrDefault();
            var userId = userIdClaim.Value;

            var userCredentialDTO = new UserCredentialDTO()
            {
                Email = email
            };

            return await BuildToken(userCredentialDTO, userId);
        }

        private async Task<AuthResponseDTO> BuildToken(UserCredentialDTO userCredentialDTO, string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentialDTO.Email),
                new Claim("id", userId)
            };

            var user = await userManager.FindByEmailAsync(userCredentialDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirtation = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expirtation, signingCredentials: creds);

            return new AuthResponseDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expirtation = expirtation
            };
        }

        [HttpPost("MakeAdmin")]
        public async Task<ActionResult> MakeAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("isAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("isAdmin", "1"));
            return NoContent();
        }
    }
}
