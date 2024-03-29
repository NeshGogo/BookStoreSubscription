﻿using Microsoft.AspNetCore.Identity;

namespace BookStoreSubscription.Entities;
public class KeyAPI
{
    public int Id { get; set; }
    public string Key { get; set; }
    public KeyType KeyType { get; set; }
    public bool Active { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public List<DomainRestriction> DomainRestrictions { get; set; }
    public List<IpRestriction> IpRestrictions { get; set; }
}

public enum KeyType
{ 
    Free = 1,
    Professional = 2,
}
