using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private List<StorageContainer> storageContainers;
    private List<task> storageContainerTasks = new List<task>();
    private KDTreeV4 storeTree;

    public void AddStorage(StorageContainer storageContainer)
    {
        storageContainers.Add(storageContainer);
        storageContainerTasks.Add(new task(1, ObjectTaskScript.TaskType.store, storageContainer.gameObject));
        storeTree = new KDTreeV4(storageContainerTasks, gridManager);
    }

    public void RemoveStorage(StorageContainer storageContainer)
    {
        storageContainers.Remove(storageContainer);
        task containerTask = storageContainerTasks.Find(x => x.obj == storageContainer.gameObject);

        if(containerTask == null)
        {
            Debug.Log("Could not find the task associated with storage container");
            return;
        }

        storageContainerTasks.Remove(containerTask);
        storeTree = new KDTreeV4(storageContainerTasks, gridManager);
    }

    public GameObject GetClosestStorage(Vector3 pos)
    {
        KDNodeV4 closestStorage = storeTree.GetClosestTask(pos);
        return closestStorage.task.obj;
    }

    //This method will start to cull the storage contianers to find the closest storage with compatible space 
    public void GetClosestStorage(List<ItemStack> items, CharacterInventory characterInventory)
    {
        //list of allowed, unallowed, unique item ids, and a tree of containers 
        List<StorageContainer> allowableStorage = new List<StorageContainer>();
        List<task> allowableStorageTasks = new List<task>();
        List<ItemStack> characterItems = new List<ItemStack>(items);
        KDTreeV4 tree;

        //find storage that allows for the item types
        foreach(StorageContainer storageContainer in storageContainers)
        {
            bool allowed = false;

            //Remove all full containers 
            if(!storageContainer.full)
            {
                allowed = true;
            }
            else
            {
                foreach (ItemStack item in characterItems)
                {
                    if (storageContainer.CheckStacks(item))
                    {
                        allowed = true;
                        break;
                    }
                }
            }
             
            //only add container to allowed list if it meets all conditions 
            if(allowed)
            {
                allowableStorage.Add(storageContainer);
            }
        }

        //If there is already no storage available
        if (allowableStorage.Count <= 0)
        {
            Debug.Log("No available storage");
            characterInventory.GetComponent<CharacterTasks>().EnabledTasks.Remove(ObjectTaskScript.TaskType.store);
            return;
        }

        //build the tasks for the tree
        foreach (StorageContainer container in allowableStorage)
        {
            allowableStorageTasks.Add(new task(1, ObjectTaskScript.TaskType.store, container.gameObject));
        }

        //build the tree
        tree = new KDTreeV4(new List<task>(allowableStorageTasks), gridManager);

        //return a list of unallowed items and the storage containers 
        KDNodeV4 closestStorage = tree.GetClosestTask(characterInventory.transform.position);

        closestStorage.task.obj.GetComponent<StorageContainer>().AddToQueue(characterItems, characterInventory);
    }

}