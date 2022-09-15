using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : ObjectTaskScript
{
    [SerializeField] private GameObject rockDrop;

    new void Start()
    {
        this.taskType = TaskType.mine;
        base.Start();
    }

    public override IEnumerator working()
    {
        yield return new WaitForSeconds(taskTime);
        GameObject tempObj =  Instantiate(rockDrop, transform.position, Quaternion.identity);
        taskManagerScript.AddTask(2, ObjectTaskScript.TaskType.haul, tempObj);
        tempObj.GetComponent<HaulScript>().SetInQueue(true);
        map.SetTile(map.WorldToCell(transform.position), null);
        Destroy(this.gameObject);
    }
}