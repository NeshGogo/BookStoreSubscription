using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs;
public class AuthorCreateDTO
{
    [Required]
    [StringLength(maximumLength: 120)]
    public string Name { get; set; }
}
