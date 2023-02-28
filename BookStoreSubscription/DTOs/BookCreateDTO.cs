using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs;
public class BookCreateDTO
{
    [StringLength(maximumLength: 250)]
    [Required]
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public List<int> AuthorIds { get; set; }
}
