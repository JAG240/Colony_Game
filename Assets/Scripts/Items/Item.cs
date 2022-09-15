using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int CurrentAmount;
    [SerializeField] private int MaxStackAmount;
    public ItemStack itemStack;
    public ItemClass itemClass;

    private void Start()
    {
        itemClass = GetComponent<ItemClass>();
        itemStack = new ItemStack(CurrentAmount, MaxStackAmount, itemClass);
    }

    public void UpdateItemStack(int CurrentAmount)
    {
        //itemStack.StackAmount = CurrentAmount;
        this.CurrentAmount = CurrentAmount;
    }
}
