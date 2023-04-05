using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs
{
    public class DomainRestrictionUpdateDTO
    {
        [Required]
        public string Domain { get; set; }
    }
}
