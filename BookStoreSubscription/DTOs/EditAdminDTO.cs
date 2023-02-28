using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs;
public class EditAdminDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
