using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace POSsystem.Core.Services;

public static class PasswordHashingService
{
    private const int HashSize = 256;

    public static string Hash(string password, byte[] salt)
    {
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 100000,
            numBytesRequested: HashSize / 8));

        return hashed;
    }
}