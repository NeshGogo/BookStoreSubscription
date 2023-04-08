using AutoMapper;
using BookStoreSubscription.DTOs;
using BookStoreSubscription.Entities;

namespace BookStoreSubscription.Helpers;
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AuthorCreateDTO, Author>();
        CreateMap<Author, AuthorDTO>();
        CreateMap<Author, AuthorWithBookDTO>()
            .ForMember(author => author.Books, opciones => opciones.MapFrom(MapAuthorDTOBooks));

        CreateMap<BookCreateDTO, Book>()
            .ForMember(book => book.AuthorBooks, opciones => opciones.MapFrom(MapAuthorsBooks));
        CreateMap<Book, BookDTO>().ReverseMap();
        CreateMap<Book, BookWithAuthorDTO>()
            .ForMember(book => book.Authors, opciones => opciones.MapFrom(MapBookDTOAuthors));
        CreateMap<BookPatchDTO, Book>().ReverseMap();

        CreateMap<CommentCreateDTO, Comment>();
        CreateMap<Comment, CommentDTO>();

        CreateMap<KeyAPI, KeyApiDTO>();
        CreateMap<KeyApiCreateDTO, KeyAPI>();

        CreateMap<DomainRestriction, DomainRestrictionDTO>();

        CreateMap<IpRestriction, IpRestrictionDTO>();
    }

    private List<BookDTO> MapAuthorDTOBooks(Author author, AuthorDTO au)
    {
        var result = new List<BookDTO>();

        if (author.AuthorBooks == null) { return result; }

        foreach (var authorBook in author.AuthorBooks)
        {
            result.Add(new BookDTO()
            {
                Id = authorBook.BookId,
                Title = authorBook.Book.Title
            });
        }

        return result;
    }

    private List<AuthorDTO> MapBookDTOAuthors(Book book, BookDTO bookDTO)
    {
        var result = new List<AuthorDTO>();

        if (book.AuthorBooks == null) { return result; }

        foreach (var authorBook in book.AuthorBooks)
        {
            result.Add(new AuthorDTO()
            {
                Id = authorBook.AuthorId,
                Name = authorBook.Author.Name
            });
        }

        return result;
    }

    private List<AuthorBook> MapAuthorsBooks(BookCreateDTO bookCreateDTO, Book book)
    {
        var result = new List<AuthorBook>();

        if (bookCreateDTO.AuthorIds == null) { return result; }

        foreach (var authorId in bookCreateDTO.AuthorIds)
        {
            result.Add(new AuthorBook() { AuthorId = authorId });
        }

        return result;
    }
}

