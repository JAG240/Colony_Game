using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreScript : ObjectTaskScript
{
    public GameObject character;
    public StorageContainer storageContainer;
    public List<ItemStack> transferItems;

    new void Start()
    {
        this.taskType = TaskType.store;
        base.Start();
    }

    public override IEnumerator working()
    {
        CharacterInventory characterInventory = character.GetComponent<CharacterInventory>();
        yield return new WaitForSeconds(taskTime);

        foreach(ItemStack stack in transferItems)
        {
            storageContainer.AddToStore(stack);
            characterInventory.RemoveFromInventory(stack);
        }

        character.GetComponent<CharacterTasks>().EnabledTasks.Add(TaskType.haul);

        Destroy(this.gameObject);
    } 
}
