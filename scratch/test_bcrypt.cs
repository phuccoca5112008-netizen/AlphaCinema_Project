using BCrypt.Net;
using System;

class Program {
    static void Main() {
        string password = "admin123";
        string hash = BCrypt.Net.BCrypt.HashPassword(password);
        bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hash}");
        Console.WriteLine($"Valid: {isValid}");
    }
}
