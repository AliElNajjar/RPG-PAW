using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBundle
{
    public readonly string Name;
    public GameObject itemObject;
    public int Amount { get; private set; }
    public bool IsEmpty { get { return Amount == 0; } }

    public ItemBundle(string name, int amount = 1, GameObject itemObject = null)
    {
        ThrowNegativeAmounts(amount);
        Name = name;
        Amount = amount;
        this.itemObject = itemObject;
    }

    private static void ThrowNegativeAmounts(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("ItemStack does not support negative amount", "amount");
    }

    public void Change(int amount)
    {
        int result = Amount + amount;
        ThrowNegativeAmounts(result);
        Amount = result;
    }

    public void Set(int amount)
    {
        ThrowNegativeAmounts(amount);
        Amount = amount;
    }

    public override string ToString()
    {
        return string.Format("{0} {1}", Amount, Name);
    }

    public ItemBundle Clone()
    {
        return new ItemBundle(Name, Amount);
    }
}
