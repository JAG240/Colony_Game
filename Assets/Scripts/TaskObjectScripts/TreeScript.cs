using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : ObjectTaskScript
{
    [SerializeField] private GameObject treeDrop;

    new void Start()
    {
        this.taskType = TaskType.chop;
        base.Start();
    }

    override public IEnumerator working()
    {
        yield return new WaitForSeconds(taskTime);
        GameObject tempObj = Instantiate(treeDrop, transform.position, Quaternion.identity);
        taskManagerScript.AddTask(2, ObjectTaskScript.TaskType.haul, tempObj);
        tempObj.GetComponent<HaulScript>().SetInQueue(true);
        map.SetTile(map.WorldToCell(transform.position), null);
        Destroy(this.gameObject);
    }
}
