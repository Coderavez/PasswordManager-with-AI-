using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.Json;

public class VaultService
{
    private readonly string filePath = "vault.dat";
    private byte[] salt;

    public List<Account> Load(string password)
    {
        if (!File.Exists(filePath))
        {
            salt = RandomNumberGenerator.GetBytes(16);
            File.WriteAllBytes(filePath, salt);
            return new List<Account>();
        }

        byte[] file = File.ReadAllBytes(filePath);

        salt = new byte[16];
        System.Array.Copy(file, 0, salt, 0, 16);

        if (file.Length == 16)
            return new List<Account>();

        byte[] encrypted = new byte[file.Length - 16];
        System.Array.Copy(file, 16, encrypted, 0, encrypted.Length);

        var key = CryptoService.GenerateKey(password, salt);

        try
        {
            string json = CryptoService.Decrypt(encrypted, key);
            return JsonSerializer.Deserialize<List<Account>>(json);
        }
        catch
        {
            return null;
        }
    }

    public void Save(List<Account> accounts, string password)
    {
        var key = CryptoService.GenerateKey(password, salt);
        string json = JsonSerializer.Serialize(accounts);

        byte[] encrypted = CryptoService.Encrypt(json, key);

        byte[] result = new byte[16 + encrypted.Length];
        System.Array.Copy(salt, 0, result, 0, 16);
        System.Array.Copy(encrypted, 0, result, 16, encrypted.Length);

        File.WriteAllBytes(filePath, result);
    }
}