using AutoMapper;
using BookStoreSubscription.DTOs;
using BookStoreSubscription.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreSubscription.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public BooksController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet("{id:int}", Name = "GetBook")]
    public async Task<ActionResult<BookWithAuthorDTO>> Get(int id)
    {
        var book = await context.Books
            .Include(book => book.AuthorBooks)
            .ThenInclude(authorBook => authorBook.Author)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        book.AuthorBooks = book.AuthorBooks.OrderBy(x => x.Order).ToList();

        return mapper.Map<BookWithAuthorDTO>(book);
    }

    [HttpPost]
    public async Task<ActionResult> Post(BookCreateDTO bookCreateDTO)
    {
        if (bookCreateDTO.AuthorIds == null)
        {
            return BadRequest("Can't add a book without author.");
        }

        var authorIds = await context.Authors
            .Where(author => bookCreateDTO.AuthorIds.Contains(author.Id)).Select(x => x.Id).ToListAsync();

        if (bookCreateDTO.AuthorIds.Count != authorIds.Count)
        {
            return BadRequest("One of the author sent not exists.");
        }

        var book = mapper.Map<Book>(bookCreateDTO);
        AsignarOrdenAutores(book);

        context.Add(book);
        await context.SaveChangesAsync();

        var bookDTO = mapper.Map<BookDTO>(book);

        return CreatedAtRoute("GetBook", new { id = book.Id }, bookDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, BookCreateDTO bookCreateDTO)
    {
        var book = await context.Books
            .Include(x => x.AuthorBooks)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        book = mapper.Map(bookCreateDTO, book);

        AsignarOrdenAutores(book);

        await context.SaveChangesAsync();
        return NoContent();
    }

    private void AsignarOrdenAutores(Book book)
    {
        if (book.AuthorBooks != null)
        {
            for (int i = 0; i < book.AuthorBooks.Count; i++)
            {
                book.AuthorBooks[i].Order = i;
            }
        }

    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
    {
        if (patchDocument == null)
        {
            return BadRequest();
        }

        var book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        var bookDTO = mapper.Map<BookPatchDTO>(book);

        patchDocument.ApplyTo(bookDTO, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

        var esValido = TryValidateModel(bookDTO);

        if (!esValido)
        {
            return BadRequest(ModelState);
        }

        mapper.Map(bookDTO, book);

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var exists = await context.Books.AnyAsync(x => x.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        context.Remove(new Book() { Id = id });
        await context.SaveChangesAsync();
        return NoContent();
    }
}

