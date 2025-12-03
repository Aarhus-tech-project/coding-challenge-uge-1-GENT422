public class Boxer
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