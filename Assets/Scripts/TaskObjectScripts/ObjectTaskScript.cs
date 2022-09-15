using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectTaskScript : MonoBehaviour
{
    [SerializeField] public string ID;
    [SerializeField] private bool inQueue = false;
    public float taskTime = 2f;
    public TaskType taskType { get; set; }
    public TaskManagerScript taskManagerScript { get; private set; }
    public Tilemap map { get; set; }

    public enum TaskType
    {
        none,
        chop,
        mine,
        build,
        haul,
        store
    }

    public void Start()
    {
        SetID();
        taskManagerScript = GameObject.Find("TaskManager").GetComponent<TaskManagerScript>();
    }

    public void SetID()
    {
        ID = taskType.ToString() + "." + transform.position.x + "." + transform.position.y;
    }

    public void SetInQueue(bool state)
    {
        inQueue = state;
    }

    public bool GetInQueue()
    {
        return inQueue;
    }

    public void StartWorking()
    {
        StartCoroutine(working());
    }

    public void StartWorking(float skillLevel)
    {
        taskTime = taskTime * (1 / skillLevel);
        StartCoroutine(working());
    }

    public virtual IEnumerator working()
    {
        yield return new WaitForSeconds(taskTime);
        Destroy(this.gameObject);
    }
}
