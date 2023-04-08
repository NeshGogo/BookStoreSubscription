using AutoMapper;
using BookStoreSubscription.DTOs;
using BookStoreSubscription.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookStoreSubscription.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class IpRestrictionsController : CustomBaseController
{
    private readonly ApplicationDbContext context;

    public IpRestrictionsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] IpRestrictionCreateDTO createDTO)
    {
        var keyDb = await context.KeyAPIs.FirstOrDefaultAsync(x => x.Id == createDTO.KeyAPIId);

        if (keyDb == null)        
            return NotFound();

        var userId = GetUserId();
        if (keyDb.UserId != userId)
            return Forbid();
        var restriction = new IpRestriction
        {
            IP = createDTO.IP,
            KeyAPIId = createDTO.KeyAPIId,
        };
        context.Add(restriction);
        await context.SaveChangesAsync();
        return NoContent(); 
    }

    [HttpPut("{id:int}")] // api/ipRestrictions/1 
    public async Task<ActionResult> Put(IpRestrictionUpdateDTO updateDTO, int id)
    {
        var ipRestriction = await context.IpRestrictions
            .Include(x => x.KeyAPI)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ipRestriction == null)
            return NotFound();

        var userId = GetUserId();
        if (ipRestriction.KeyAPI.UserId != userId)
            return Forbid();
        ipRestriction.IP = updateDTO.IP;
        context.Update(ipRestriction);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")] // api/ipRestrictions/2
    public async Task<ActionResult> Delete(int id)
    {
        var ipRestriction = await context.IpRestrictions
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ipRestriction == null)
            return NotFound();

        var userId = GetUserId();
        if (ipRestriction.KeyAPI.UserId != userId)
            return Forbid();

        context.Remove(ipRestriction);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

