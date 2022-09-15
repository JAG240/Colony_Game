using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPriorityDistanceComparer : IComparer<task>
{
    public int Compare(task t1, task t2)
    {
        if (t1.priority.CompareTo(t2.priority) < 0)
        {
            return -1;
        }
        else if (t1.priority.CompareTo(t2.priority) == 0)
        {
            if (t1.distanceFromTask.CompareTo(t2.distanceFromTask) < 0)
            {
                return -1;
            }
            else if (t1.distanceFromTask.CompareTo(t2.distanceFromTask) == 0)
            {
                return t1.distanceFromTask.CompareTo(t2.distanceFromTask);
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return 1;
        }
    }
}
