using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTasks : MonoBehaviour
{
    public List<task> tasks = new List<task>();
    public List<ObjectTaskScript.TaskType> EnabledTasks = new List<ObjectTaskScript.TaskType>() { ObjectTaskScript.TaskType.chop, ObjectTaskScript.TaskType.mine};
    public bool working = false;
    public CharacterMovement charMove;
    private PathFinder pathFinder = new PathFinder();
    private GridManager gridManager;
    private TaskManagerScript taskManager;
    private CharacterAttributes characterAttributes;
    public Toggle chopToggle;
    public Toggle mineToggle;

    void Start()
    {
        charMove = this.gameObject.GetComponent<CharacterMovement>();
        characterAttributes = this.gameObject.GetComponent<CharacterAttributes>();
        this.gridManager = charMove.gridManager;
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManagerScript>();
    }

    void Update()
    {
        doTasks();
    }

    private void doTasks()
    {
        if (tasks.Count > 0 && !working && !charMove.GetMoving())
        {
            if (tasks[0].taskType == ObjectTaskScript.TaskType.haul)
            {
                HaulScript haulScript = tasks[0].obj.GetComponent<HaulScript>();
                haulScript.character = this.gameObject;
            }

            float skill = characterAttributes.charClass.Skills.ContainsKey(tasks[0].taskType) ? characterAttributes.charClass.Skills[tasks[0].taskType] : 1;

            if ((Vector3)tasks[0].obj.transform.position != charMove.targetPos)
            {
                charMove.targetPos = new Vector2(tasks[0].obj.transform.position.x, tasks[0].obj.transform.position.y);
            }

            if (charMove.GetNearTaskObject())
            {
                working = true;
                tasks[0].obj.GetComponent<ObjectTaskScript>().StartWorking(skill);
            }
        }
        else if (working)
        {
            if (tasks[0].obj == null)
            {
                tasks.Remove(tasks[0]);
                working = false;
            }
        }
    }

    public void AddTask(task newTask, List<Spot> Path)
    {
        tasks.Add(newTask);
        charMove.targetPos = newTask.obj.transform.position;
        Path.RemoveAt(Path.Count - 1);
        charMove.targetPath = Path;
    }

    public string getTaskList()
    {
        string taskList = string.Empty;
        foreach (task t in tasks)
        {
            taskList += t.taskType + ", " + t.assignee.transform.name + ", " + t.obj.transform.name + ", " + t.obj.transform.position + ", " + transform.position + "\n";
        }
        return taskList;
    }
    public void EnableTask(ObjectTaskScript.TaskType taskType)
    {
        if(!EnabledTasks.Contains(taskType))
        {
            EnabledTasks.Add(taskType);
        }
    }

    public void DisableTask(ObjectTaskScript.TaskType taskType)
    {
        if(EnabledTasks.Contains(taskType))
        {
            EnabledTasks.Remove(taskType);
        }
    }

    public void SetListeners()
    {
        chopToggle.onValueChanged.AddListener(delegate { if (chopToggle.isOn) { EnableTask(ObjectTaskScript.TaskType.chop); } else { DisableTask(ObjectTaskScript.TaskType.chop); } });
        mineToggle.onValueChanged.AddListener(delegate { if (mineToggle.isOn) { EnableTask(ObjectTaskScript.TaskType.mine); } else { DisableTask(ObjectTaskScript.TaskType.mine); } });
    }
}
