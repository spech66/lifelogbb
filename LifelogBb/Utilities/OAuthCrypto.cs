using System.Security.Cryptography;
using System.Text;

namespace LifelogBb.Utilities;

/// <summary>
/// Secret generation, storage hashing and PKCE verification for the OAuth server.
/// </summary>
public static class OAuthCrypto
{
    /// <summary>Creates a base64url encoded random secret. 32 bytes is 256 bits of entropy.</summary>
    public static string NewSecret(int byteCount = 32) => Base64Url(RandomNumberGenerator.GetBytes(byteCount));

    /// <summary>
    /// Hash for the stored copy of a code or refresh token. These are high entropy random values,
    /// so a preimage resistant hash is enough. A password hash would only make the indexed lookup
    /// impossible without buying any additional protection.
    /// </summary>
    public static string Sha256Hex(string value) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.ASCII.GetBytes(value)));

    /// <summary>
    /// RFC 7636 S256: BASE64URL(SHA256(ASCII(code_verifier))) has to equal the stored challenge.
    /// </summary>
    public static bool VerifyPkceS256(string codeVerifier, string storedChallenge)
    {
        var computed = Base64Url(SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier)));
        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(computed), Encoding.ASCII.GetBytes(storedChallenge));
    }

    /// <summary>RFC 7636 unreserved character set for verifiers and challenges.</summary>
    public static bool IsValidPkceValue(string? value)
    {
        if (value is null || value.Length < OAuthDefaults.MinPkceLength || value.Length > OAuthDefaults.MaxPkceLength)
        {
            return false;
        }

        foreach (var c in value)
        {
            var allowed = c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '-' or '.' or '_' or '~';
            if (!allowed)
            {
                return false;
            }
        }

        return true;
    }

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
