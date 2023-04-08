namespace BookStoreSubscription.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public bool Payed { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime PaydayLimitDate { get; set; }
    }
}
