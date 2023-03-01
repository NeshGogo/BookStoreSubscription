using Microsoft.AspNetCore.Identity;

namespace BookStoreSubscription.Entities;
public class KeyAPI
{
    public int Id { get; set; }
    public string Key { get; set; }
    public KeyType KeyType { get; set; }
    public bool Active { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}

public enum KeyType
{ 
    Free = 1,
    Professional = 2,
}
