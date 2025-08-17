using System;
using System.Linq;

public static class InviteCodeGenerator
{
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static Random _random = new Random();

    public static string GenerateCode(int length = 6)
    {
        return new string(Enumerable.Repeat(_chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
