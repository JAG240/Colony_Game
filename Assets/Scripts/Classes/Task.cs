using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class task
{
    public int priority { get; set; }
    public ObjectTaskScript.TaskType taskType { get; set; }
    public GameObject obj { get; set; }
    public GameObject assignee { get; set; }
    public float distanceFromTask { get; set; }

    public task(int priority, ObjectTaskScript.TaskType taskType, GameObject obj)
    {
        this.priority = priority;
        this.taskType = taskType;
        this.obj = obj;
    }
}
