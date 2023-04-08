using Microsoft.AspNetCore.Identity;

namespace BookStoreSubscription.Entities
{
    public class User : IdentityUser
    {
        public bool BadPayer { get; set; }
    }
}
