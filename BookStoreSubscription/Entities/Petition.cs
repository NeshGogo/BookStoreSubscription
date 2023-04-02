namespace BookStoreSubscription.Entities
{
    public class Petition
    {
        public int Id { get; set; }
        public int KeyAPIId { get; set; }
        public DateTime PetitionDate { get; set; }
        public KeyAPI KeyAPI { get; set; }
    }
}
