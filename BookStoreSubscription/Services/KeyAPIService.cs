using BookStoreSubscription.Entities;

namespace BookStoreSubscription.Services;
public class KeyAPIService
{
    private readonly ApplicationDbContext dbContext;

    public KeyAPIService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Add(string userId, KeyType keyType)
    {
        var key = Guid.NewGuid().ToString().Replace("-", "");
        var entity = new KeyAPI
        {
            UserId = userId,
            KeyType = keyType,
            Active = true,
            Key = key
        };
        await dbContext.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }
}

