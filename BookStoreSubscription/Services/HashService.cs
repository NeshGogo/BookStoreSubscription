using BookStoreSubscription.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace BookStoreSubscription.Services;
public class HashService
{
    public HashResultDTO Hash(string plainText)
    {
        var sal = new byte[16];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(sal);
        }

        return Hash(plainText, sal);
    }

    public HashResultDTO Hash(string plainText, byte[] sal)
    {
        var llaveDerivada = KeyDerivation.Pbkdf2(password: plainText,
            salt: sal, prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 32);

        var hash = Convert.ToBase64String(llaveDerivada);

        return new HashResultDTO()
        {
            Hash = hash,
            Sal = sal
        };
    }
}

