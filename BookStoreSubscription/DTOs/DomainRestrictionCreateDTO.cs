using System.ComponentModel.DataAnnotations;

namespace BookStoreSubscription.DTOs
{
    public class DomainRestrictionCreateDTO
    {
        public int KeyAPIId { get; set; }
        [Required]
        public string Domain { get; set; }
    }
}
