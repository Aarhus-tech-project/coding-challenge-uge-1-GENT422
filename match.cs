public static class Match 
{
      static readonly string[] leftIdle = new[] { "  O  ", " /|\\ ", " / \\ " };
    static readonly string[] leftPunch = new[] { "  O  ", " -|> ", " / \\ " };
    static readonly string[] leftHit = new[] { "  O  ", " -|- ", " / \\ " };
    static readonly string[] rightIdle = new[] { "  O  ", " /|\\ ", " / \\ " };
    static readonly string[] rightPunch = new[] { "  O  ", " <|- ", " / \\ " };
    static readonly string[] rightHit = new[] { "  O  ", " -|- ", " / \\ " };
    static readonly string koArt = @"  x_x
 /|\
 / \";

   public static void ShowArena(Boxer p1, Boxer p2)

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
static void TryClear()
    {
    try { Console.Clear(); } catch { }
       
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
  public static void ShowArenaWithActions(Boxer p1, Boxer p2, string a1, string a2, bool h1, bool h2)
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

}