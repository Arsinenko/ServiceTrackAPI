namespace AuthApp.application.Interfaces;

public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password with a randomly generated salt
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>A string containing the salt and hash in format: "salt:hash"</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a stored hash
    /// </summary>
    /// <param name="storedHash">The stored hash in format "salt:hash"</param>
    /// <param name="providedPassword">The password to verify</param>
    /// <returns>True if the password matches the hash, false otherwise</returns>
    bool VerifyHashedPassword(string storedHash, string providedPassword);
}