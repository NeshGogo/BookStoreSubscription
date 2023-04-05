using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStoreSubscription.Services;
using BookStoreSubscription.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BookStoreSubscription.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class KeyApisController : CustomBaseController
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;
        private readonly KeyAPIService keyAPIService;

        public KeyApisController(IMapper mapper, ApplicationDbContext context, KeyAPIService keyAPIService)
        {
            this.mapper = mapper;
            this.context = context;
            this.keyAPIService = keyAPIService;
        }

        [HttpGet]
        public async Task<ActionResult<List<KeyApiDTO>>> Get()
        {
            var userId = GetUserId();
            var keys = await context.KeyAPIs
                .Include(x => x.DomainRestrictions)
                .Include(x => x.IpRestrictions)
                .Where(x => x.UserId == userId).ToListAsync();
            return mapper.Map<List<KeyApiDTO>>(keys);
        }

        [HttpPost]
        public async Task<ActionResult> Post(KeyApiCreateDTO createDTO)
        {
            var userId = GetUserId();
            if(createDTO.keyType == Entities.KeyType.Free)
            {
                var alreadyHadAFreeKey = await context.KeyAPIs.AnyAsync(x => x.UserId == userId && x.KeyType == Entities.KeyType.Free);
                if (alreadyHadAFreeKey)
                    return BadRequest("The user already had a free key");
            }

            await keyAPIService.Add(userId, createDTO.keyType);
            return NoContent();
        }


        [HttpPut]
        public async Task<ActionResult> Put(KeyApiUpdateDTO updateDTO)
        {
            var userId = GetUserId();
            var key = await context.KeyAPIs.FirstOrDefaultAsync(x => x.Id == updateDTO.KeyId);
            if (key == null) return NotFound();
            if (userId != key.UserId) return Forbid();
            if (updateDTO.UpdateKey)
                key.Key = keyAPIService.GenerateKey();

            key.Active = updateDTO.Active;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
