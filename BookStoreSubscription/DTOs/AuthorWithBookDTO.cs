namespace BookStoreSubscription.DTOs
{
    public class AuthorWithBookDTO : AuthorDTO
    {
        public List<BookDTO> Books { get; set; }
    }
}
