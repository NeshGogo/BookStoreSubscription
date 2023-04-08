using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs;
public class IpRestrictionCreateDTO
{
    public int KeyAPIId { get; set; }
    [Required]
    public string IP { get; set; }
}
