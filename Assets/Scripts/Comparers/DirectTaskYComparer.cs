using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectTaskYComparer : IComparer<task>
{
    public int Compare(task x, task y)
    {
        return (int)x.obj.transform.position.y - (int)y.obj.transform.position.y;
    }
}
