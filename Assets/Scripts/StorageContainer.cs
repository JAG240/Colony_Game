using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageContainer : MonoBehaviour
{
    [SerializeField] public List<int> allowableItemIDs;
    [SerializeField] private int maxStorageSlots;
    public bool full = false;
    private List<ItemStack> currentStorage = new List<ItemStack>();
    private StorageClass storageClass;
    private StorageManager storageManager;
    private List<ItemStack> queuedItems = new List<ItemStack>();
    private bool queuing = false;
    private bool storing = false;
    private Queue<IEnumerator> itemQueue = new Queue<IEnumerator>();
    private Queue<IEnumerator> storeQueue = new Queue<IEnumerator>();

    private void Start()
    {
        storageManager = GameObject.Find("StorageManager").GetComponent<StorageManager>();
        storageManager.AddStorage(this);
        storageClass = new StorageClass(allowableItemIDs, currentStorage, ref maxStorageSlots);
    }

    //this method will only allow storing of queued items 
    public void AddToStore(ItemStack itemStack)
    {
        storeQueue.Enqueue(Store(new ItemStack(itemStack.StackAmount, itemStack.MaxStackAmount, itemStack.itemClass)));

        if(!storing)
        {
            StartCoroutine(StartStore());
        }
    }

    private IEnumerator StartStore()
    {
        storing = true;
        yield return new WaitWhile(() => queuing);

        while(storeQueue.Count > 0)
        {
            StartCoroutine(storeQueue.Dequeue());
        }

        storing = false;
    }

    //This will remove from queue and store the item
    private IEnumerator Store(ItemStack itemStack)
    {
        //remove the item from the queue
        int addAmount = itemStack.StackAmount;

        foreach (ItemStack stack in queuedItems)
        {
            if (addAmount == 0)
                break;

            if(stack.itemClass.ID == itemStack.itemClass.ID)
            {
                addAmount = stack.RemoveFromStack(addAmount);
            }
        }

        //clears the queue of all empty stacks 
        for(int i = 0; i < queuedItems.Count; i++)
        {
            if(queuedItems[i].StackAmount == 0)
            {
                queuedItems.RemoveAt(i);
                i--;
            }
        }

        //actually stores the item 
        storageClass.Store(itemStack);
        yield return null;
    }

    //print the storage to debug
    public void DebugInv()
    {
        if(currentStorage.Count < 1)
        {
            Debug.Log("There is nothing in the chest");
            return;
        }

        foreach(ItemStack stack in currentStorage)
        {
            Debug.Log(stack.itemClass.Name + " : " + stack.StackAmount + "/" + stack.MaxStackAmount);
        }
    }

    //this checks all stacks for if the item can be combined at all 
    public bool CheckStacks(ItemStack itemStack)
    {
        //this checks current storage
        foreach(ItemStack stack in currentStorage)
        {
            if(stack.itemClass.ID == itemStack.itemClass.ID)
            {
                if(stack.StackAmount < stack.MaxStackAmount)
                {
                    return true;
                }
            }
        }

        //this checks queued storage
        foreach(ItemStack stack in queuedItems)
        {
            if(stack.itemClass.ID == itemStack.itemClass.ID)
            {
                if(stack.StackAmount < stack.MaxStackAmount)
                {
                    return true;
                }
            }
        }

        //if there are no stack with space at all, it cannot be done
        return false;
    }

    //this will add the item stack to the queue and start the queue if not started 
    public void AddToQueue(List<ItemStack> itemStacks, CharacterInventory characterInventory)
    {
        //queue the item stack check and assign a return value 
        itemQueue.Enqueue(Queue(itemStacks, characterInventory));

        //start the queue if not started
        if(!queuing)
        {
            StartCoroutine(StartQueue());
        }
    }

    //this will continue to queue items in the chest if there is queued items 
    private IEnumerator StartQueue()
    {
        queuing = true;
        yield return new WaitWhile(() => storing);

        while (itemQueue.Count > 0)
        {
            yield return StartCoroutine(itemQueue.Dequeue());
        }

        queuing = false;
    }

    //this method takes the items stack and returns a list of items that were queued and the container for task creation then checks if its full after the queue 
    private IEnumerator Queue(List<ItemStack> itemStacks, CharacterInventory characterInventory)
    {
        List<ItemStack> storedItems = new List<ItemStack>();
        List<ItemStack> storeItems = itemStacks.ConvertAll(item => item.Copy());
        List<ItemStack> tempQueued = queuedItems.ConvertAll(item => item.Copy());
        List<ItemStack> totalItems = currentStorage.ConvertAll(item => item.Copy());

        //populates a combined list of all queued items and stored items. Makes sure to compress stacks when possible.
        foreach (ItemStack item in tempQueued)
        {
            //checks if we can compress stacks 
            foreach(ItemStack stack in totalItems)
            {
                if (item.StackAmount == 0)
                    break;

                if(stack.itemClass.ID == item.itemClass.ID && stack.StackAmount < stack.MaxStackAmount)
                {
                    if(item.StackAmount + stack.StackAmount > stack.MaxStackAmount)
                    {
                        item.StackAmount = stack.AddToStack(item.StackAmount);
                    }
                    else
                    {
                        stack.AddToStack(item.StackAmount);
                        item.StackAmount = 0;
                    }
                }
            }

            //if there is a remaining ammount add it to the total storage
            if(item.StackAmount > 0)
            {
                totalItems.Add(item);
            }
        }

        /*Debug.Log("Current: ");
        foreach(ItemStack item in currentStorage)
        {
            Debug.Log(item.itemClass.Name + ": " + item.StackAmount + "/" + item.MaxStackAmount);
        }

        Debug.Log("Queued: ");
        foreach (ItemStack item in queuedItems)
        {
            Debug.Log(item.itemClass.Name + ": " + item.StackAmount + "/" + item.MaxStackAmount);
        }

        Debug.Log("Total:");
        foreach (ItemStack item in totalItems)
        {
            Debug.Log(item.itemClass.Name + ": " + item.StackAmount + "/" + item.MaxStackAmount);
        }*/

        //find if there are any stacks that will combine and removes them from the item stack 
        foreach (ItemStack item in storeItems)
        {
            if (full)
                break;

            //attempts to combine stacks from the total storage if possible 
            foreach (ItemStack stack in totalItems)
            {
                if(item.itemClass.ID == stack.itemClass.ID && stack.StackAmount < stack.MaxStackAmount)
                {
                    if (item.StackAmount == 0)
                        break;

                    if(item.StackAmount + stack.StackAmount > stack.MaxStackAmount)
                    {
                        int remainder = item.StackAmount - (stack.MaxStackAmount - stack.StackAmount);
                        int addedAmount = item.StackAmount - remainder;
                        item.StackAmount = remainder;
                        storedItems.Add(new ItemStack(addedAmount, item.MaxStackAmount, item.itemClass));
                        stack.StackAmount = stack.MaxStackAmount;
                    }
                    else
                    {
                        storedItems.Add(new ItemStack(item.StackAmount, item.MaxStackAmount, item.itemClass));
                        stack.AddToStack(item.StackAmount);
                        item.StackAmount = 0;
                    }
                }
            }

            //if there is no compatiable stack, check if we have a slot to put it in. If not, the item is not added to the stored list.
            if(item.StackAmount > 0)
            {
                if(totalItems.Count < maxStorageSlots)
                {
                    storedItems.Add(new ItemStack(item.StackAmount, item.MaxStackAmount, item.itemClass));
                    totalItems.Add(new ItemStack(item.StackAmount, item.MaxStackAmount, item.itemClass));
                    item.StackAmount = 0;
                }
            }

            //checks if there is space to continue adding items to chest
            if(totalItems.Count >= maxStorageSlots)
            {
                if(totalItems.Count > maxStorageSlots)
                    full = true;
                else
                {
                    bool internalFull = true;

                    foreach (ItemStack stack in totalItems)
                    {
                        if (stack.StackAmount < stack.MaxStackAmount)
                        {
                            internalFull = false;
                            break;
                        }
                    }

                    full = internalFull;
                }
            }
        }

        //compress stored list after removals 
        for (int i = 0; i < storedItems.Count; i++)
        {
            for(int j = 0; j < i; j++)
            {
                //compare i to j aka compare the current stack to every stack before it 
                if(storedItems[i].itemClass.ID == storedItems[j].itemClass.ID && storedItems[j].StackAmount < storedItems[j].MaxStackAmount)
                {
                    if(storedItems[i].StackAmount + storedItems[j].StackAmount > storedItems[j].MaxStackAmount)
                    {
                        storedItems[i].StackAmount = storedItems[i].StackAmount - (storedItems[j].MaxStackAmount - storedItems[j].StackAmount);
                        storedItems[j].StackAmount = storedItems[j].MaxStackAmount;
                    }
                    else
                    {
                        storedItems[j].AddToStack(storedItems[i].StackAmount);
                        storedItems[i].StackAmount = 0;
                    }
                }

                //remove the item from the stack if it is 0
                if (storedItems[i].StackAmount == 0)
                    storedItems.RemoveAt(i);
            }
        }

        //add stored items to queued list and compress 
        foreach (ItemStack item in storedItems)
        {
            int addAmount = item.StackAmount;

            for(int i = 0; i < queuedItems.Count; i++)
            {
                if (item.itemClass.ID == queuedItems[i].itemClass.ID && queuedItems[i].StackAmount < queuedItems[i].MaxStackAmount)
                {
                    if (addAmount == 0)
                        break;

                    if (item.StackAmount + queuedItems[i].StackAmount > queuedItems[i].MaxStackAmount)
                    {
                        addAmount = queuedItems[i].AddToStack(addAmount);
                        queuedItems[i].StackAmount = queuedItems[i].MaxStackAmount;
                    }
                    else
                    {
                        queuedItems[i].AddToStack(item.StackAmount);
                        addAmount = 0;
                    }
                }
            }

            if(addAmount > 0)
            {
                queuedItems.Add(new ItemStack(addAmount, item.MaxStackAmount, item.itemClass));
            }
        }

        //add task with the stored items to the character inventory 
        characterInventory.AddStoreTask(storedItems, this.gameObject);

        Debug.Log("Stored Items: ");
        foreach (ItemStack item in storedItems)
        {
            Debug.Log(item.itemClass.Name + ": " + item.StackAmount + "/" + item.MaxStackAmount);
        }

        yield return null;
    }
}