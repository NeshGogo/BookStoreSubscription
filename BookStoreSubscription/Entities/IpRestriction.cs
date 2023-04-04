namespace BookStoreSubscription.Entities;
public class IpRestriction
{
  public int Id { get; set; }
  public int KeyAPIId { get; set; }
  public string IP { get; set; }
  public KeyAPI KeyAPI { get; set; }
}