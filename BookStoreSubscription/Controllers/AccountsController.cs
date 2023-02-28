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
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public AccountsController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("unique value and maybe secret");
        }

        [HttpGet("hash/{plainText}")]
        public ActionResult RealizarHash(string plainText)
        {
            var result1 = hashService.Hash(plainText);
            var result2 = hashService.Hash(plainText);
            return Ok(new
            {
                plainText = plainText,
                Hash1 = result1,
                Hash2 = result2
            });
        }

        [HttpGet("encrypt")]
        public ActionResult Encriptar()
        {
            var plainText = "Rafael Aguero";
            var encryptedText = dataProtector.Protect(plainText);
            var unencrypted = dataProtector.Unprotect(encryptedText);

            return Ok(new
            {
                textoPlano = plainText,
                textoCifrado = encryptedText,
                textoDesencriptado = unencrypted
            });
        }

        [HttpGet("encryptByTime")]
        public ActionResult EncriptarPorTiempo()
        {
            var timeLimitedDataProtector = dataProtector.ToTimeLimitedDataProtector();

            var plainText = "Rafael Aguero";
            var encryptedText = timeLimitedDataProtector.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var unencrypted = timeLimitedDataProtector.Unprotect(encryptedText);

            return Ok(new
            {
                textoPlano = plainText,
                textoCifrado = encryptedText,
                textoDesencriptado = unencrypted
            });
        }

        [HttpPost("register")] // api/accounts/register
        public async Task<ActionResult<AuthResponseDTO>> Registrar(UserCredentialDTO userCredentialDTO)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialDTO.Email,
                Email = userCredentialDTO.Email
            };
            var result = await userManager.CreateAsync(user, userCredentialDTO.Password);

            if (result.Succeeded)
            {
                return await ConstruirToken(userCredentialDTO);
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
                return await ConstruirToken(userCredentialDTO);
            }
            else
            {
                return BadRequest("Bad login");
            }
        }

        [HttpGet("refreshToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthResponseDTO>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var userCredentialDTO = new UserCredentialDTO()
            {
                Email = email
            };

            return await ConstruirToken(userCredentialDTO);
        }

        private async Task<AuthResponseDTO> ConstruirToken(UserCredentialDTO userCredentialDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentialDTO.Email),
                new Claim("what I want", "whatever other value")
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
        public async Task<ActionResult> HacerAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("isAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditAdminDTO editAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("isAdmin", "1"));
            return NoContent();
        }
    }
}
