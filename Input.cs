using System;
using System.Runtime.InteropServices;
using System.Threading;

public static class Input
{
    private static readonly Random rnd = new Random();

    public static string Read()
    {
        string? line = Console.ReadLine();
        return line == null ? "" : line.Trim();
    }

    public static void SlowWrite(string text, int delay = 10)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }

    public static void Clear()
    {
        try { Console.Clear(); } catch { }
    }

    public static void PlayHitSound()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try { Console.Beep(600, 80); Console.Beep(400, 60); }
            catch { }
        }
    }

    public static void PlayKO()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try { Console.Beep(200, 250); Console.Beep(120, 300); }
            catch { }
        }
    }

    public static void PlayMiss()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try { Console.Beep(300, 120); }
            catch { }
        }
    }

    public static string GetAction(Boxer b, bool vsCpu)
    {
        // CPU logik
        if (vsCpu && b.Name == "CPU")
        {
            if (b.Stamina < 10 && rnd.Next(100) < 60) return "rest";
            if (rnd.Next(100) < 40) return "jab";
            if (rnd.Next(100) < 70) return "hook";
            return "uppercut";
        }

        while (true)
        {
            Console.WriteLine("Vælg handling:");
            Console.WriteLine("[1] Jab        [2] Hook       [3] Uppercut");
            Console.WriteLine("[4] Blok       [5] Hvil");
            Console.Write("> ");

            string input = Read();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string action = input switch
            {
                "1" => "jab",
                "2" => "hook",
                "3" => "uppercut",
                "4" => "block",
                "5" => "rest",
                _ => null
            };
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            if (action != null)
                return action;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ugyldigt valg! Prøv igen.\n");
            Console.ResetColor();
        }
    }
}
  