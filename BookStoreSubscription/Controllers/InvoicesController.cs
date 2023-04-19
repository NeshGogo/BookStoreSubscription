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
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InvoicesController : CustomBaseController
{
    private readonly ApplicationDbContext context;

    public InvoicesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] PayInvoiceDTO createDTO)
    {
        var invoiceDb = await context.Invoices
            .Include(x => User)
            .FirstOrDefaultAsync(x => x.Id == createDTO.InvoiceId);

        if (invoiceDb == null)        
            return NotFound();
        if (invoiceDb.Payed)
            return BadRequest("the invoice has been already paid");

        // TODO logic to pay the bill with a pay gateway.

        invoiceDb.Payed = true;
        
        var existsPendingInvoicesExpired = await context.Invoices
            .AnyAsync(x => x.UserId == invoiceDb.UserId && 
            !x.Payed && x.PaydayLimitDate < DateTime.Today);
       
        if (!existsPendingInvoicesExpired)
            invoiceDb.User.BadPayer = false;

        await context.SaveChangesAsync();
        return NoContent(); 
    }
}

