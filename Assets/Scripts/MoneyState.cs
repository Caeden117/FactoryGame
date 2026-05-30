using System;

public static class MoneyState
{
    public static int Money {get; private set;}
    public static event Action<int> MoneyChanged;
       
    public static void Add(int amount)
    {
        if(amount <= 0)
        {
            return;
        }

        Money += amount;
        MoneyChanged?.Invoke(Money);
    }

    public static void Reset(int startingMoney = 0)
    {
        Money = startingMoney;
        MoneyChanged?.Invoke(Money);
    }
}