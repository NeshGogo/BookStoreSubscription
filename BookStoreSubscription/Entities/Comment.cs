using Microsoft.AspNetCore.Identity;

namespace BookStoreSubscription.Entities;
public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}
