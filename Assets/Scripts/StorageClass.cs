using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageClass
{
    public List<int> allowableItemIDs { get; set; }
    public List<ItemStack> currentStorage { get; set; }
    private int maxStorageSlots { get; set; }

    public StorageClass()
    {
        this.allowableItemIDs = new List<int>();
        this.currentStorage = new List<ItemStack>();
        this.maxStorageSlots = 0;
    }

    public StorageClass(List<int> allowableItemIDs, List<ItemStack> currentStorage, ref int maxStorageSlots)
    {
        this.allowableItemIDs = allowableItemIDs;
        this.currentStorage = currentStorage;
        this.maxStorageSlots = maxStorageSlots;
    }

    //Adds item stack to storage 
    public int Store(ItemStack itemStack)
    {
        //Count to add amount 
        int addAmount = itemStack.StackAmount;

        //checks if this item is not allowed to be in the container
        if (!allowableItemIDs.Contains(itemStack.itemClass.ID))
            return addAmount;

        //Attempt to combine stacks of exisiting items 
        if (currentStorage.Count > 0)
        {
            foreach(ItemStack stack in currentStorage)
            {
                if(stack.itemClass.ID == itemStack.itemClass.ID)
                {
                    addAmount = stack.AddToStack(addAmount);
                }

                if(addAmount == 0)
                    break;
            }
        }

        //Create new stack if needed 
        //Later verision will check if item slot 
        if(addAmount > 0)
        {
            currentStorage.Add(new ItemStack(addAmount, itemStack.MaxStackAmount, itemStack.itemClass));
        }

        return addAmount;
    }

    //Used to remove items from storage
    public int Remove()
    {
        return 0;
    }
}
