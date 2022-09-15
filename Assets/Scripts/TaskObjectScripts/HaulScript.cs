using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaulScript : ObjectTaskScript
{
    public GameObject character { get; set; }
    public Item item { get; set; }

    new void Start()
    {
        this.taskType = TaskType.haul;
        item = gameObject.GetComponent<Item>();
        base.Start();
    }

    public override IEnumerator working()
    {
        yield return new WaitForSeconds(taskTime);
        CharacterInventory characterInventory = character.GetComponent<CharacterInventory>();

        //add to inventory
        int remainingAmount = characterInventory.AddToInventory(item.itemStack);

        if(remainingAmount > 0)
        {
            GameObject tempObj = Instantiate(this.gameObject, transform.position, Quaternion.identity);
            tempObj.GetComponent<Item>().UpdateItemStack(remainingAmount);
            taskManagerScript.AddTask(2, TaskType.haul, tempObj);
        }

        Destroy(this.gameObject);
    }
}
