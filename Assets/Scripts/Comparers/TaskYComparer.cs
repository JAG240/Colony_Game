using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskYComparer : IComparer<Vector2>
{
    public int Compare(Vector2 v1, Vector2 v2)
    {
        if (v1.y.CompareTo(v2.y) < 0)
        {
            return -1;
        }
        else if (v1.y.CompareTo(v2.y) == 0)
        {
            return v1.y.CompareTo(v2.y);
        }
        else
        {
            return 1;
        }
    }
}
