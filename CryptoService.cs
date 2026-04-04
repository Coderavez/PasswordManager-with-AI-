using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class CryptoService
{
    public static byte[] GenerateKey(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256
        );

        return pbkdf2.GetBytes(32);
    }

    public static byte[] Encrypt(string text, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);

        sw.Write(text);
        sw.Close();

        return ms.ToArray();
    }

    public static string Decrypt(byte[] data, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;

        byte[] iv = new byte[16];
        System.Array.Copy(data, 0, iv, 0, 16);
        aes.IV = iv;

        using var ms = new MemoryStream(data, 16, data.Length - 16);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}