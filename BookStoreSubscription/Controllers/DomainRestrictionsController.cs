using AutoMapper;
using BookStoreSubscription.DTOs;
using BookStoreSubscription.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreSubscription.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DomainRestrictionsController : CustomBaseController
{
    private readonly ApplicationDbContext context;

    public DomainRestrictionsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] DomainRestrictionCreateDTO createDTO)
    {
        var keyDb = await context.KeyAPIs.FirstOrDefaultAsync(x => x.Id == createDTO.KeyAPIId);

        if (keyDb == null)        
            return NotFound();

        var userId = GetUserId();
        if (keyDb.UserId != userId)
            return Forbid();
        var restriction = new DomainRestriction
        {
            Domain = createDTO.Domain,
            KeyAPIId = createDTO.KeyAPIId,
        };
        context.Add(restriction);
        await context.SaveChangesAsync();
        return NoContent(); 
    }

    [HttpPut("{id:int}")] // api/DomainRestrictions/1 
    public async Task<ActionResult> Put(DomainRestrictionUpdateDTO updateDTO, int id)
    {
        var domainRestriction = await context.DomainRestrictions
            .Include(x => x.KeyAPI)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (domainRestriction == null)
            return NotFound();

        var userId = GetUserId();
        if (domainRestriction.KeyAPI.UserId != userId)
            return Forbid();
        domainRestriction.Domain = updateDTO.Domain;
        context.Update(domainRestriction);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")] // api/DomainRestrictions/2
    public async Task<ActionResult> Delete(int id)
    {
        var domainRestriction = await context.DomainRestrictions
            .FirstOrDefaultAsync(x => x.Id == id);

        if (domainRestriction == null)
            return NotFound();

        var userId = GetUserId();
        if (domainRestriction.KeyAPI.UserId != userId)
            return Forbid();

        context.Remove(domainRestriction);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

