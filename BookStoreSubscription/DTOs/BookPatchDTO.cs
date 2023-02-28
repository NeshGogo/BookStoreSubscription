using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs;
public class BookPatchDTO
{
    [StringLength(maximumLength: 250)]
    [Required]
    public string Title { get; set; }
    public DateTime PublishedDate { get; set; }
}
