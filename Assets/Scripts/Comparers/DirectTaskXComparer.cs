using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectTaskXComparer : IComparer<task>
{
    public int Compare(task x, task y)
    {
        return (int)x.obj.transform.position.x - (int)y.obj.transform.position.x;
    }
}
