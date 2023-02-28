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
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;

    public AuthorsController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
    {
        this.context = context;
        this.mapper = mapper;
        this.configuration = configuration;
    }

    [HttpGet] // api/Authors
    [AllowAnonymous]
    public async Task<List<AuthorDTO>> Get()
    {
        var authors = await context.Authors.ToListAsync();
        return mapper.Map<List<AuthorDTO>>(authors);
    }

    [HttpGet("{id:int}", Name = "GetAuthor")]
    public async Task<ActionResult<AuthorWithBookDTO>> Get(int id)
    {
        var author = await context.Authors
            .Include(autorDB => autorDB.AuthorBooks)
            .ThenInclude(autorLibroDB => autorLibroDB.Book)
            .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        return mapper.Map<AuthorWithBookDTO>(author);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<List<AuthorDTO>>> Get([FromRoute] string name)
    {
        var authors = await context.Authors.Where(a => a.Name.Contains(name)).ToListAsync();

        return mapper.Map<List<AuthorDTO>>(authors);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] AuthorCreateDTO authorCreateDTO)
    {
        var existsWithSameName = await context.Authors.AnyAsync(x => x.Name == authorCreateDTO.Name);

        if (existsWithSameName)
        {
            return BadRequest($"Already exists an author with the name {authorCreateDTO.Name}");
        }

        var author = mapper.Map<Author>(authorCreateDTO);

        context.Add(author);
        await context.SaveChangesAsync();

        var autorDTO = mapper.Map<AuthorDTO>(author);

        return CreatedAtRoute("GetAuthor", new { id = author.Id }, autorDTO);
    }

    [HttpPut("{id:int}")] // api/Authors/1 
    public async Task<ActionResult> Put(AuthorCreateDTO authorCreateDTO, int id)
    {
        var existe = await context.Authors.AnyAsync(x => x.Id == id);

        if (!existe)
        {
            return NotFound();
        }

        var author = mapper.Map<Author>(authorCreateDTO);
        author.Id = id;

        context.Update(author);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")] // api/autores/2
    public async Task<ActionResult> Delete(int id)
    {
        var exists = await context.Authors.AnyAsync(x => x.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        context.Remove(new Author() { Id = id });
        await context.SaveChangesAsync();
        return NoContent();
    }
}

