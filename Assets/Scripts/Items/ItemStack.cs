using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack
{
    public int StackAmount;
    public int MaxStackAmount;
    public ItemClass itemClass;

    public ItemStack()
    {
        StackAmount = 0;
        MaxStackAmount = 0;
        itemClass = null;
    }

    public ItemStack(int StackAmount, int MaxStackAmount, ItemClass itemClass)
    {
        this.StackAmount = StackAmount;
        this.MaxStackAmount = MaxStackAmount;
        this.itemClass = itemClass;
    }

    public ItemStack Copy()
    {
        return new ItemStack(this.StackAmount, this.MaxStackAmount, this.itemClass);
    }

    public int RemoveFromStack(int Amount)
    {
        if(StackAmount - Amount >= 0)
        {
            StackAmount -= Amount;
            return 0;
        }
        else
        {
            int remainder = Amount - StackAmount;
            StackAmount = 0;
            return remainder;
        }
    }

    public int AddToStack(int Amount)
    {
        if(StackAmount + Amount <= MaxStackAmount)
        {
            StackAmount += Amount;
            return 0;
        }
        else
        {
            int remainder = Amount - (MaxStackAmount - StackAmount);
            StackAmount = MaxStackAmount;
            return remainder;
        }
    }
}
