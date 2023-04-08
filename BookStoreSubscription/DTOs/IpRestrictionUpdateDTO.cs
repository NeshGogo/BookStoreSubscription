using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs
{
    public class IpRestrictionUpdateDTO
    {
        [Required]
        public string IP { get; set; }
    }
}
