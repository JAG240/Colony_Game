using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    [SerializeField] private GameObject storePrefab;
    private CharacterAttributes characterAttributes;
    private StorageManager storageManager;
    [SerializeField] private float carryWeight;
    private float maxCarryWeight;
    public List<ItemStack> Items = new List<ItemStack>();
    private CharacterTasks characterTasks;
    private CharacterMovement characterMovement;

    private void Start()
    {
        characterAttributes = GetComponent<CharacterAttributes>();
        maxCarryWeight = characterAttributes.charClass.Attributes["carryWeight"] * 100;
        carryWeight = maxCarryWeight;
        storageManager = GameObject.Find("StorageManager").GetComponent<StorageManager>();
        characterTasks = characterAttributes.GetComponent<CharacterTasks>();
        characterMovement = characterAttributes.GetComponent<CharacterMovement>();
    }

    //replace void return type int so that stacks are not unlimited 
    public int AddToInventory(ItemStack itemStack)
    {
        //getting the item weight and setting up the remaining amount
        float itemWeight = itemStack.itemClass.weight;
        int remainingAmount = 0;

        //Add amount is used as a overflow for stacks on when to create stacks 
        int addAmount = itemStack.StackAmount;

        if(carryWeight - (itemStack.StackAmount * itemWeight) < 0)
        {
            addAmount = Mathf.FloorToInt(carryWeight / itemWeight);
            remainingAmount = itemStack.StackAmount - addAmount;
        }

        carryWeight -= addAmount * itemWeight;

        //checking if items can be combined into a stack 
        foreach (ItemStack stack in Items)
        {
            if(itemStack.itemClass.ID == stack.itemClass.ID && addAmount > 0)
            {
                addAmount = stack.AddToStack(addAmount);
            }
            else if(addAmount == 0)
            {
                break;
            }
        }

        //making a new stack if needed AKA there is a remaining stack amount
        if(addAmount > 0)
        {
            Items.Add(new ItemStack(addAmount, itemStack.MaxStackAmount, itemStack.itemClass));
        }

        //if we cannot pick up another item then stop attempting to pick up items 
        if(remainingAmount > 0)
        {
            GetStorageContainer();
        }

        return remainingAmount;
    }

    //removes stuff from inventory 
    public void RemoveFromInventory(ItemStack itemStack)
    {
        int addAmount = itemStack.StackAmount;

        foreach(ItemStack stack in Items)
        {
            if(itemStack.itemClass.ID == stack.itemClass.ID)
            {
                addAmount = stack.RemoveFromStack(addAmount);
            }

            if (addAmount == 0)
                break;
        }

        for(int i = 0; i < Items.Count; i++)
        {
            if(Items[i].StackAmount <= 0)
            {
                Items.RemoveAt(i);
                i--;
            }
        }

        carryWeight += itemStack.StackAmount * itemStack.itemClass.weight;
    }

    //Prints inventory to debug nice and pretty 
    public void DebugInventory()
    {
        foreach(ItemStack stack in Items)
        {
            Debug.Log(characterAttributes.transform.name + " - " + stack.itemClass.Name + ": " + stack.StackAmount + "/" + stack.MaxStackAmount);
        }
    }

    //This method will reach out to torage manager to get start the task of getting a container to store into
    private void GetStorageContainer()
    {
        //stop picking stuff up
        characterAttributes.GetComponent<CharacterTasks>().EnabledTasks.Remove(ObjectTaskScript.TaskType.haul);

        //find the closest storage of the allowed items
        storageManager.GetClosestStorage(Items, this);
    }

    //this is method that makes a store task for this character 
    public void AddStoreTask(List<ItemStack> itemList, GameObject container)
    {
        //create logic to get make the store task and make it theh top task

        if(itemList.Count == 0)
        {
            return;
        }

        GameObject newStoreTask = Instantiate(storePrefab, container.transform.position, Quaternion.identity);
        task storeTask = new task(1, ObjectTaskScript.TaskType.store, newStoreTask);

        StoreScript newTaskStoreScript = newStoreTask.GetComponent<StoreScript>();
        newTaskStoreScript.character = characterAttributes.gameObject;
        newTaskStoreScript.storageContainer = container.GetComponent<StorageContainer>();
        newTaskStoreScript.transferItems = itemList.ConvertAll(item => item.Copy());

        characterTasks.AddTask(storeTask, characterMovement.GetPathTo(container.transform.position));
    }
}
