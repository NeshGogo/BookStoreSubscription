namespace BookStoreSubscription.Entities;
public class DomainRestriction
{
  public int Id { get; set; }
  public int KeyAPIId { get; set; }
  public string Domain { get; set; }
  public KeyAPI KeyAPI { get; set; }
}