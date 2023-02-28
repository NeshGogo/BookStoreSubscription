using AutoMapper;
using BookStoreSubscription.DTOs;
using BookStoreSubscription.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreSubscription.Controllers;

[Route("api/books/{bookId:int}/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly UserManager<IdentityUser> userManager;

    public CommentsController(ApplicationDbContext context,
        IMapper mapper,
        UserManager<IdentityUser> userManager)
    {
        this.context = context;
        this.mapper = mapper;
        this.userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
    {
        var exists = await context.Books.AnyAsync(libroDB => libroDB.Id == bookId);

        if (!exists)
        {
            return NotFound();
        }

        var comments = await context.Comments
            .Where(comment => comment.BookId == bookId).ToListAsync();

        return mapper.Map<List<CommentDTO>>(comments);
    }

    [HttpGet("{id:int}", Name = "GetComment")]
    public async Task<ActionResult<CommentDTO>> GetPorId(int id)
    {
        var comentario = await context.Comments.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

        if (comentario == null)
        {
            return NotFound();
        }

        return mapper.Map<CommentDTO>(comentario);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post(int bookId, CommentCreateDTO commentCreateDTO)
    {
        var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
        var email = emailClaim.Value;
        var user = await userManager.FindByEmailAsync(email);
        var userId = user.Id;
        var existsBook = await context.Books.AnyAsync(book => book.Id == bookId);

        if (!existsBook)
        {
            return NotFound();
        }

        var comentario = mapper.Map<Comment>(commentCreateDTO);
        comentario.BookId = bookId;
        comentario.UserId = userId;
        context.Add(comentario);
        await context.SaveChangesAsync();

        var comentarioDTO = mapper.Map<CommentDTO>(comentario);

        return CreatedAtRoute("GetComment", new { id = comentario.Id, bookId = bookId }, comentarioDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int bookId, int id, CommentCreateDTO commentCreateDTO)
    {
        var exists = await context.Books.AnyAsync(book => book.Id == bookId);

        if (!exists)
        {
            return NotFound();
        }

        var existsComment = await context.Comments.AnyAsync(comment => comment.Id == id);

        if (!existsComment)
        {
            return NotFound();
        }

        var comentario = mapper.Map<Comment>(commentCreateDTO);
        comentario.Id = id;
        comentario.BookId = bookId;
        context.Update(comentario);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

