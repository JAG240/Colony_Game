using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildObjScript : ObjectTaskScript
{
    public Builder builder;
    public Tile tile;

    new void Start()
    {
        taskType = TaskType.build;
        base.Start();
        taskManagerScript.AddTask(1, TaskType.build, this.gameObject);
    }

    public override IEnumerator working()
    {
        yield return new WaitForSeconds(taskTime);
        builder.PlaceTile(transform.position, tile);
        Destroy(this.gameObject);
    }
}
