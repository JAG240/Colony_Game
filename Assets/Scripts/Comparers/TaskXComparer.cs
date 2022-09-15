using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskXComparer : IComparer<Vector2>
{
    public int Compare(Vector2 v1, Vector2 v2)
    {
        if(v1.x.CompareTo(v2.x) < 0)
        {
            return -1;
        }
        else if(v1.x.CompareTo(v2.x) == 0)
        {
            return v1.x.CompareTo(v2.x);
        }
        else
        {
            return 1;
        }
    }
}
