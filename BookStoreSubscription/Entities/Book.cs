using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.Entities;
public class Book
{
    public int Id { get; set; }
    [Required]
    [StringLength(maximumLength: 250)]
    public string Title { get; set; }
    public DateTime? PublishedDate { get; set; }
    public List<Comment> Comments { get; set; }
    public List<AuthorBook> AuthorBooks { get; set; }
}
