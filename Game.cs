using System;

public static class Game
{
    static readonly Random rnd = new Random();

    public static void StartGame()
    {
        Console.Title = "Boksekamp – Deluxe Edition";
        bool playing = true;

        while (playing)
        {
            Input.Clear();
            Console.WriteLine("BOKSEKAMP – DELUXE EDITION");
            Console.WriteLine("[1] 2 spillere");
            Console.WriteLine("[2] Spil mod CPU");
            Console.WriteLine("[3] Afslut");
            Console.Write("> ");
            string choice = Input.Read();

            if (choice == "3")
            {
                playing = false;
                break;
            }

            bool vsCpu = choice == "2";

            Console.Write("\nNavn på spiller 1: ");
            string p1Name = Input.Read();
            if (string.IsNullOrWhiteSpace(p1Name)) p1Name = "Spiller 1";

            string p2Name;
            if (vsCpu) p2Name = "CPU";
            else
            {
                Console.Write("Navn på spiller 2: ");
                p2Name = Input.Read();
                if (string.IsNullOrWhiteSpace(p2Name)) p2Name = "Spiller 2";
            }

            Boxer p1 = new Boxer(p1Name, rnd);
            Boxer p2 = new Boxer(p2Name, rnd);

            RunFight(p1, p2);

        
        }
    }

    static void RunFight(Boxer p1, Boxer p2)
    {
        int round = 1;

        while (p1.IsAlive && p2.IsAlive)
        {
            Input.Clear();
            Match.ShowArena(p1, p2);

            Console.WriteLine($"\n╔══ Runde {round} ══╗");
            Console.WriteLine("Begge boksere vælger handling...\n");

            string action1 = GetPlayerAction(p1, false);
            string action2 = GetPlayerAction(p2, p2.Name == "CPU");

            var result1 = PerformAction(p1, action1, p2);
            var result2 = PerformAction(p2, action2, p1);

            Match.ShowArenaWithActions(p1, p2, action1, action2, result1.Hit, result2.Hit);

            ShowResults(p1, p2, action1, result1);
            ShowResults(p2, p1, action2, result2);

            p1.ReduceStun();
            p2.ReduceStun();
            round++;

            Console.WriteLine("\nTryk ENTER for næste runde…");
            Console.ReadLine();
        }

        Input.Clear();
        if (p1.IsAlive) Input.SlowWrite($"{p1.Name} vinder kampen!", 12);
        else Input.SlowWrite($"{p2.Name} vinder kampen!", 12);
        Input.PlayKO();

        Console.WriteLine("\nTryk ENTER for at gå tilbage til menuen…");
        Console.ReadLine();
    }

    static string GetPlayerAction(Boxer b, bool vsCpu)
    {
        if (b.IsStunned)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Input.SlowWrite($"{b.Name} er tilendebragt!", 10);
            Console.ResetColor();
            b.ReduceStun();
            return "none";
        }

        string action = Input.GetAction(b, vsCpu);

        if (action == "rest") { Input.SlowWrite($"{b.Name} hviler…", 10); b.Rest(); }
        else if (action == "block") { Input.SlowWrite($"{b.Name} går i blok!", 10); b.StartBlock(); }
        else { Console.ForegroundColor = ConsoleColor.Magenta; Input.SlowWrite($"{b.Name} forbereder: {action.ToUpper()}!", 8); Console.ResetColor(); }

        return action;
    }

    static (bool Hit, int Damage, bool KO) PerformAction(Boxer attacker, string action, Boxer defender)
    {
        if (attacker.IsStunned || (action != "jab" && action != "hook" && action != "uppercut"))
            return (false, 0, false);

        return attacker.Attack(action, defender);
    }

    static void ShowResults(Boxer attacker, Boxer defender, string action, (bool Hit, int Damage, bool KO) result)
    {
        if (result.Hit)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Input.SlowWrite($"✓ {attacker.Name} ramte {defender.Name} for {result.Damage} skade!", 12);
            Console.ResetColor();
            Input.PlayHitSound();
            if (result.KO)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Input.SlowWrite($"{defender.Name} KNOCKED OUT!", 25);
                Console.ResetColor();
                Console.WriteLine(Match.koArt);
                Input.PlayKO();
            }
        }
        else if (action == "jab" || action == "hook" || action == "uppercut")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Input.SlowWrite($"✗ {attacker.Name} missede!", 12);
            Console.ResetColor();
            Input.PlayMiss();
        }
    }
}
  