using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManagerScript : MonoBehaviour
{
    #region Vars
    private GameObject[] characters;
    private List<CharacterTasks> charTasks = new List<CharacterTasks>();
    private PathFinder pathFinder = new PathFinder();
    private bool assigningTasks = false;
    [SerializeField] private GridManager gridManager;

    private KDTreeV4 NulledKDTree;
    private KDTreeV4 ChopKDTree;
    private KDTreeV4 MineKDTree;
    private KDTreeV4 HaulKDTree;
    private KDTreeV4 BuildKDTree;

    //[SerializeField] private GameObject testTaskObj;
    #endregion

    void Start()
    {
        characters = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject character in characters)
        {
            charTasks.Add(character.GetComponent<CharacterTasks>());
        }

        #region KDTree Intialization 
        NulledKDTree = new KDTreeV4(new List<task>(), gridManager);
        ChopKDTree = new KDTreeV4(new List<task>(), gridManager);
        MineKDTree = new KDTreeV4(new List<task>(), gridManager);
        HaulKDTree = new KDTreeV4(new List<task>(), gridManager);
        BuildKDTree = new KDTreeV4(new List<task>(), gridManager);
        #endregion

        #region Testing Cases
        //KDTree Test cases
        /*GameObject tempTask1 = Instantiate(testTaskObj, new Vector3(1f, -2f, 0f), Quaternion.identity);
        List<task> testTask = new List<task>();
        task newTestTask1 = new task(1, ObjectTaskScript.TaskType.mine, tempTask1);
        testTask.Add(newTestTask1);
        KDTree kDTree = new KDTree(testTask);
        task resultTask = kDTree.GetBestTask(new Vector2(-10f, 17f));
        Debug.Log("Closest task: " + resultTask.obj.transform.position);*/
        #endregion
    }


    void Update()
    {
        //If there are tasks to do and tasks are not being assigned, start assigning tasks 
        if(!assigningTasks && (ChopKDTree.taskList.Count > 0 || MineKDTree.taskList.Count > 0 || HaulKDTree.taskList.Count > 0 || BuildKDTree.taskList.Count > 0))
        {
            AssignTasks();
        }
    }

    //This method will add a single task to a KDTree by remaking the KDTree with the new task included 
    public void AddTask(int priority, ObjectTaskScript.TaskType taskType, GameObject obj)
    {
        //Gets a reference to the tree needed
        ref KDTreeV4 activeTree = ref GetActiveTree(taskType);

        //Makes a task out of the new item
        task newTask = new task(priority, taskType, obj);
        
        //if not defaulted, make a new tree with the new task added
        if(activeTree != NulledKDTree)
        {
            List<task> currentTasks = new List<task>(activeTree.taskList);
            currentTasks.Add(newTask);
            activeTree = new KDTreeV4(currentTasks, gridManager);
        }
    }

    //This bulk loads many tasks at once
    public void AddTasks(List<task> tasks, ObjectTaskScript.TaskType taskType)
    {
        ref KDTreeV4 activeTree = ref GetActiveTree(taskType);

        //loops through tasks and adds new task to task list then rebuilds tree
        if (activeTree != NulledKDTree)
        {
            List<task> currentTasks = new List<task>(activeTree.taskList);

            foreach (task t in tasks)
            {
                if (!currentTasks.Contains(t))
                {
                    currentTasks.Add(t);
                }
            }

            activeTree = new KDTreeV4(currentTasks, gridManager);
        }
    }

    private void AssignTasks()
    {
        //So that this does not run multiple times 
        assigningTasks = true;

        //Loops through the characters task list and assigns task if enabled 
        foreach (CharacterTasks taskList in charTasks)
        {
            //vars used to add tasks to character 
            List<Spot> path = null;
            task closestTask = null;

            //Only work with characters that have no current task and have tasks enabled
            if (taskList.tasks.Count == 0 && taskList.EnabledTasks.Count > 0)
            {
                //loops through the enabled tasks and find the closest one, order the enabled task list for a priority 
                for(int x = 0; x < taskList.EnabledTasks.Count; x++)
                {
                    ref KDTreeV4 activeTree = ref GetActiveTree(taskList.EnabledTasks[x]);

                    if(activeTree.taskList.Count > 0)
                    {
                        KDNodeV4 closestNode = activeTree.GetClosestTask(taskList.transform.position);
                        closestTask = closestNode.task;
                        Spot taskPos = gridManager.GetSpot(closestTask.obj.transform.position);
                        Spot characterPos = gridManager.GetSpot(taskList.transform.position);
                        path = pathFinder.GetPath(characterPos, taskPos);
                        activeTree.RemoveNode(closestNode);
                        break;
                    }
                }

                //If the task is reachable 
                if (path != null)
                {
                    taskList.AddTask(closestTask, new List<Spot>(path));
                    closestTask.assignee = taskList.gameObject;
                }
            }
        }

        //Allows tasks to be assigned if more are added
        assigningTasks = false;
    }

    //this method takes a task type and returns a reference to the KD tree of that type 
    private ref KDTreeV4 GetActiveTree(ObjectTaskScript.TaskType taskType)
    {
        switch (taskType)
        {
            case ObjectTaskScript.TaskType.chop:
                return ref ChopKDTree;
            case ObjectTaskScript.TaskType.mine:
                return ref MineKDTree;
            case ObjectTaskScript.TaskType.haul:
                return ref HaulKDTree;
            case ObjectTaskScript.TaskType.build:
                return ref BuildKDTree;
            default:
                return ref NulledKDTree;
        }
    }
}