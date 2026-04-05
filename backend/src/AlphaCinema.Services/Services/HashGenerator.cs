
using System;

namespace AlphaCinema.Services.Services;

public static class HashGenerator {
    public static void Main() {
        string password = "admin123";
        string hash = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine("HASH:" + hash);
    }
}
