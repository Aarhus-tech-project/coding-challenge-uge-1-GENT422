using System;
using System.Threading;
using System.Runtime.InteropServices;

class Boxer
{
    public string Name { get; private set; }
    public int MaxHP { get; private set; }
    public int HP { get; private set; }
    public int Stamina { get; private set; }
    public bool IsStunned { get; private set; }
    public bool IsBlocking { get; private set; }

    private int stunTurns = 0;
    private readonly Random rnd;

    public Boxer(string name, Random random, int maxHp = 120, int startStamina = 60)
    {
        Name = name;
        rnd = random;
        MaxHP = maxHp;
        HP = maxHp;
        Stamina = startStamina;
        IsStunned = false;
        IsBlocking = false;
    }

    public bool IsAlive => HP > 0;

    public void StartBlock()
    {
        if (Stamina >= 5)
        {
            Stamina -= 5;
            IsBlocking = true;
        }
    }

    public void EndBlock() => IsBlocking = false;

    public void Rest()
    {
        Stamina += 18;
        if (Stamina > 100) Stamina = 100;
    }

    public void TakeDamage(int dmg)
    {
        HP -= dmg;
        if (HP < 0) HP = 0;
    }

    public void ApplyStun()
    {
        IsStunned = true;
        stunTurns = 1;
    }

    public void ReduceStun()
    {
        if (IsStunned)
        {
            stunTurns--;
            if (stunTurns <= 0)
                IsStunned = false;
        }
    }

    public (bool Hit, int Damage, bool KO) Attack(string type, Boxer enemy)
    {
        int staminaCost;
        int minDamage;
        int maxDamage;
        int hitChance;

        switch (type)
        {
            case "uppercut":
                staminaCost = 18; minDamage = 16; maxDamage = 32; hitChance = 55;
                break;

            case "hook":
                staminaCost = 12; minDamage = 10; maxDamage = 20; hitChance = 70;
                break;

            default:
                staminaCost = 8; minDamage = 6; maxDamage = 12; hitChance = 85;
                break;
        }

        if (Stamina < staminaCost) return (false, 0, false);

        Stamina -= staminaCost;

        if (rnd.Next(100) >= hitChance)
        {
            return (false, 0, false);
        }

        int dmg = rnd.Next(minDamage, maxDamage + 1);

        if (enemy.IsBlocking)
        {
            dmg /= 2;
            enemy.EndBlock();
        }

        bool knockout = dmg >= (int)(enemy.MaxHP * 0.3);
        if (knockout) enemy.ApplyStun();

        enemy.TakeDamage(dmg);

        return (true, dmg, knockout);
    }

    public override string ToString() => $"{Name} | HP: {HP}/{MaxHP} | STA: {Stamina}";
}

class Program
{
    static readonly Random rnd = new Random();

    static readonly string[] leftIdle = new[] { "  O  ", " /|\\ ", " / \\ " };
    static readonly string[] leftPunch = new[] { "  O  ", " -|> ", " / \\ " };
    static readonly string[] leftHit = new[] { "  O  ", " -|- ", " / \\ " };
    static readonly string[] rightIdle = new[] { "  O  ", " /|\\ ", " / \\ " };
    static readonly string[] rightPunch = new[] { "  O  ", " <|- ", " / \\ " };
    static readonly string[] rightHit = new[] { "  O  ", " -|- ", " / \\ " };
    static readonly string koArt = @"  x_x
 /|\
 / \";

    static string ReadInput()
    {
        string? line = Console.ReadLine();
        return line == null ? "" : line.Trim();
    }

    static void SlowWrite(string text, int delay = 10)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }

    static void PlayHitSound()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try { Console.Beep(600, 80); Console.Beep(400, 60); }
            catch { }
        }
    }

    static void PlayKO()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try { Console.Beep(200, 250); Console.Beep(120, 300); }
            catch { }
        }
    }

    static void PlayMissSound()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try { Console.Beep(300, 120); }
            catch { }
        }
    }

    static void TryClear()
    {
        try { Console.Clear(); } catch { }
    }

    static void ShowArena(Boxer p1, Boxer p2)
    {
        TryClear();
        int width = 70;
        try { width = Math.Min(Console.WindowWidth - 4, 100); } catch { }

        int inner = width - 4;
        int dashCount = (inner - 5) / 2;
        string arenaLine = "+" + new string('-', dashCount) + " ARENA " + new string('-', dashCount) + "+";
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(arenaLine);
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("| " + p1.Name.PadRight(inner / 2) + p2.Name.PadLeft(inner - inner / 2) + " |");
        Console.ResetColor();
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("| " + ($"HP:{p1.HP}/{p1.MaxHP} STA:{p1.Stamina}").PadRight(inner / 2) + ($"HP:{p2.HP}/{p2.MaxHP} STA:{p2.Stamina}").PadLeft(inner - inner / 2) + " |");
        Console.ResetColor();
        
        Console.WriteLine("|" + new string(' ', inner + 2) + "|");
        
        RenderBoxers(leftIdle, rightIdle, inner);
        
        Console.WriteLine("|" + new string(' ', inner + 2) + "|");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(arenaLine);
        Console.ResetColor();
    }

    static void RenderBoxers(string[] left, string[] right, int innerWidth)
    {
        int leftW = 0; foreach (var l in left) if (l.Length > leftW) leftW = l.Length;
        int rightW = 0; foreach (var r in right) if (r.Length > rightW) rightW = r.Length;
        int maxLines = Math.Max(left.Length, right.Length);

        for (int i = 0; i < maxLines; i++)
        {
            string l = i < left.Length ? left[i] : "";
            string r = i < right.Length ? right[i] : "";
            Console.Write("| ");
            Console.Write(l.PadRight(leftW));
            Console.Write(new string(' ', 10));
            Console.Write(r.PadRight(rightW));
            Console.WriteLine(" |");
        }
    }

    static void ShowArenaWithActions(Boxer p1, Boxer p2, string a1, string a2, bool h1, bool h2)
    {
        TryClear();
        int width = 70;
        try { width = Math.Min(Console.WindowWidth - 4, 100); } catch { }

        int inner = width - 4;  
        
        
        // Yellow header box
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("+" + new string('=', width - 2) + "+");
        string title = "🥊 KAMPEN TAGER FORM 🥊";

        int padding = (inner - title.Length) / 2;
        Console.WriteLine("| " + new string(' ', padding) + title + new string(' ', inner - padding - title.Length) + " |");
        Console.WriteLine("+" + new string('=', width - 2) + "+");
        Console.ResetColor();
        
        Console.WriteLine();
        
        int dashCount = (inner - 5) / 2;
        string arenaLine = "+" + new string('-', dashCount) + " ARENA " + new string('-', dashCount) + "+";
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(arenaLine);
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("| " + p1.Name.PadRight(inner / 2) + p2.Name.PadLeft(inner - inner / 2) + " |");
        Console.ResetColor();
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("| " + ($"HP:{p1.HP}/{p1.MaxHP} STA:{p1.Stamina}").PadRight(inner / 2) + ($"HP:{p2.HP}/{p2.MaxHP} STA:{p2.Stamina}").PadLeft(inner - inner / 2) + " |");
        Console.ResetColor();
        
        Console.WriteLine("|" + new string(' ', inner + 2) + "|");

        string[] lArt = leftIdle;
        if (a1 == "jab" || a1 == "hook" || a1 == "uppercut") lArt = h1 ? leftHit : leftPunch;

        string[] rArt = rightIdle;
        if (a2 == "jab" || a2 == "hook" || a2 == "uppercut") rArt = h2 ? rightHit : rightPunch;

        RenderBoxers(lArt, rArt, inner);

        Console.WriteLine("|" + new string(' ', inner + 2) + "|");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(arenaLine);
        Console.ResetColor();
    }

    static string GetAction(Boxer b, bool vsCpu)
    {
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

            string input = ReadInput();
            string action = input switch
            {
                "1" => "jab",
                "2" => "hook",
                "3" => "uppercut",
                "4" => "block",
                "5" => "rest",
                _ => null
            };

            if (action != null) return action;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ugyldigt valg! Prøv igen.\n");
            Console.ResetColor();
        }
    }

    static void Main()
    {
        Console.Title = "Boksekamp – Deluxe Edition";

        Console.WriteLine("Vælg mode:");
        Console.WriteLine("[1] 2 spillere");
        Console.WriteLine("[2] Spil mod CPU");
        Console.Write("> ");

        bool vsCpu = ReadInput() == "2";

        Console.Write("\nNavn på spiller 1: ");
        string p1Name = ReadInput();
        if (string.IsNullOrWhiteSpace(p1Name)) p1Name = "Spiller 1";

        string p2Name;
        if (vsCpu)
        {
            p2Name = "CPU";
        }
        else
        {
            Console.Write("Navn på spiller 2: ");
            p2Name = ReadInput();
            if (string.IsNullOrWhiteSpace(p2Name)) p2Name = "Spiller 2";
        }

        Boxer p1 = new Boxer(p1Name, rnd);
        Boxer p2 = new Boxer(p2Name, rnd);

        int round = 1;

        while (p1.IsAlive && p2.IsAlive)
        {
            TryClear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║            BOKSEKAMP - KONSOLEDITIONEN          ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            ShowArena(p1, p2);

            Console.WriteLine($"\n╔══ Runde {round} ══╗");
            Console.WriteLine("Begge boksere vælger handling...\n");

            string action1 = "none";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"--- {p1.Name}s handling ---");
            Console.ResetColor();

            if (p1.IsStunned)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SlowWrite($"{p1.Name} er tilendebragt!", 10);
                Console.ResetColor();
                p1.ReduceStun();
            }
            else
            {
                action1 = GetAction(p1, false);
                if (action1 == "rest")
                {
                    SlowWrite($"{p1.Name} hviler…", 10);
                    p1.Rest();
                }
                else if (action1 == "block")
                {
                    SlowWrite($"{p1.Name} går i blok!", 10);
                    p1.StartBlock();
                }
                else if (action1 == "jab" || action1 == "hook" || action1 == "uppercut")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    SlowWrite($"{p1.Name} forbereder: {action1.ToUpper()}!", 8);
                    Console.ResetColor();
                }
            }

            Console.WriteLine();

            string action2 = "none";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"--- {p2.Name}s handling ---");
            Console.ResetColor();

            if (p2.IsStunned)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SlowWrite($"{p2.Name} er tilendebragt!", 10);
                Console.ResetColor();
                p2.ReduceStun();
            }
            else
            {
                action2 = GetAction(p2, vsCpu);
                if (action2 == "rest")
                {
                    SlowWrite($"{p2.Name} hviler…", 10);
                    p2.Rest();
                }
                else if (action2 == "block")
                {
                    SlowWrite($"{p2.Name} går i blok!", 10);
                    p2.StartBlock();
                }
                else if (action2 == "jab" || action2 == "hook" || action2 == "uppercut")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    SlowWrite($"{p2.Name} forbereder: {action2.ToUpper()}!", 8);
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("ANGREBENE UDFØRES!\n");

            (bool Hit, int Damage, bool KO) result1 = (false, 0, false);
            (bool Hit, int Damage, bool KO) result2 = (false, 0, false);

            if (!p1.IsStunned && (action1 == "jab" || action1 == "hook" || action1 == "uppercut"))
            {
                result1 = p1.Attack(action1, p2);
            }

            if (!p2.IsStunned && (action2 == "jab" || action2 == "hook" || action2 == "uppercut"))
            {
                result2 = p2.Attack(action2, p1);
            }

            ShowArenaWithActions(p1, p2, action1, action2, result1.Hit, result2.Hit);

            if (result1.Hit)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                SlowWrite($"✓ {p1.Name} ramte {p2.Name} for {result1.Damage} skade!", 12);
                Console.ResetColor();
                PlayHitSound();
                if (result1.KO)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    SlowWrite($"{p2.Name} KNOCKED OUT!", 25);
                    Console.ResetColor();
                    Console.WriteLine(koArt);
                    PlayKO();
                    break;
                }
            }
            else if (action1 == "jab" || action1 == "hook" || action1 == "uppercut")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                SlowWrite($"✗ {p1.Name} missede!", 12);
                Console.ResetColor();
                PlayMissSound();
            }

            if (result2.Hit)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                SlowWrite($"✓ {p2.Name} ramte {p1.Name} for {result2.Damage} skade!", 12);
                Console.ResetColor();
                PlayHitSound();
                if (result2.KO)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    SlowWrite($"{p1.Name} KNOCKED OUT!", 25);
                    Console.ResetColor();
                    Console.WriteLine(koArt);
                    PlayKO();
                    break;
                }
            }
            else if (action2 == "jab" || action2 == "hook" || action2 == "uppercut")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                SlowWrite($"✗ {p2.Name} missede!", 12);
                Console.ResetColor();
                PlayMissSound();
            }

            p1.ReduceStun();
            p2.ReduceStun();
            round++;

            Console.WriteLine("\nTryk ENTER for næste runde…");
            Console.ReadLine();
        }

        TryClear();
        Console.ForegroundColor = ConsoleColor.Green;
        if (p1.IsAlive) SlowWrite($"{p1.Name} vinder kampen!", 12);
        else SlowWrite($"{p2.Name} vinder kampen!", 12);
        Console.ResetColor();
        PlayKO();
    }
}
  