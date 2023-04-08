namespace BookStoreSubscription.DTOs
{
    public class KeyApiDTO
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool Active { get; set; }
        public string KeyType { get; set; }
        public List<DomainRestrictionDTO> DomainRestrictions { get; set; }
        public List<IpRestrictionDTO> IpRestrictions { get; set; }
    }
}
