using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs;
public class UserCredentialDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
