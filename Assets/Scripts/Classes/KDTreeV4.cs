using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class will take in a task list and construct a KDTree with all needed function to find the clostest tasks as well as remove tasks. 
public class KDTreeV4 
{
    #region Vars

    public List<task> taskList = new List<task>();
    private int k = 2;
    private int x = 0;
    private int y = 1;
    private KDNodeV4 rootNode;
    private GridManager gridManager;
    private PathFinder pathFinder = new PathFinder();

    #endregion

    #region Constructor

    //initial constructor with required grid manager
    public KDTreeV4(List<task> taskList, GridManager gridManager)
    {
        if(gridManager == null)
        {
            Debug.Log("Grid Manager in KDTree is null: Please set Grid Manager with costructor new KDTreeV4(gridManager)");
            return;
        }

        this.gridManager = gridManager;
        this.taskList = new List<task>(taskList);
        rootNode = Buildtree(taskList, 0);
    }

    #endregion

    #region Methods

    //build tree is recursive to build the entire tree
    private KDNodeV4 Buildtree(List<task> taskList, int depth)
    {
        if (taskList.Count < 1)
            return null;

        //axis refers tp the index at which we will sort in k dimensions. (x,y) = [0,1] for our 2 dimensional tree
        int axis = depth % k;

        //sorts based on axis
        if(axis == x)
            taskList.Sort(new DirectTaskXComparer());
        else if(axis == y)
            taskList.Sort(new DirectTaskYComparer());
        else
            Debug.Log("Axis is not 0 or 1");

        //Finding the task in the middle, the position of that task, and if this is the only task assigning it or null if not 
        task medianTask = taskList[Mathf.FloorToInt(taskList.Count / 2)];
        Vector2 medianPoint = new Vector2(medianTask.obj.transform.position.x, medianTask.obj.transform.position.y);
        task task = taskList.Count == 1 ? medianTask : null;

        //If there are more than 1 task, create a list of task for right and left else just make empty lists 
        List<task> leftTask = taskList.Count > 1 ? taskList.GetRange(0, Mathf.FloorToInt(taskList.Count / 2f)) : new List<task>();
        List<task> rightTask = taskList.Count > 1 ? taskList.GetRange(Mathf.FloorToInt(taskList.Count / 2f), Mathf.CeilToInt(taskList.Count / 2f)): new List<task>();

        //if there is a task left build children else dont
        KDNodeV4 leftChild = leftTask.Count > 0 ? Buildtree(leftTask, depth + 1) : null;
        KDNodeV4 rightChild = rightTask.Count > 0 ? Buildtree(rightTask, depth + 1) : null;

        //Build our current node 
        KDNodeV4 currentNode = new KDNodeV4(axis, medianPoint, leftChild, rightChild, task);

        //If children were made, make the current node the parent 
        if(leftChild != null)
            leftChild.parent = currentNode;

        if(rightChild != null)
            rightChild.parent = currentNode;

        //default to return the current node 
        return currentNode;
    }

    //Public method that can be called on other scripts 
    public KDNodeV4 GetFirstTask(Vector2 position)
    {
        return GetFirstTask(rootNode, position);
    }

    //private methiod to recursively walk tree and return first node 
    private KDNodeV4 GetFirstTask(KDNodeV4 currentNode, Vector2 position)
    {
        //If a task is found return it 
        if(currentNode.task != null)
            return currentNode;

        //Using the axis to decided which path (left or right)
        if (currentNode.axis == x)
            currentNode = position.x < currentNode.medianPoint.x ? currentNode.leftChild : currentNode.rightChild;
        else
            currentNode = position.y < currentNode.medianPoint.y ? currentNode.leftChild : currentNode.rightChild;

        //recursive call
        return GetFirstTask(currentNode, position);
    }

    public KDNodeV4 GetClosestTask(Vector2 position)
    {
        KDNodeV4 firstNode = GetFirstTask(position);
        KDNodeV4 bestNode = GetClosestTask(position, firstNode, firstNode, new List<KDNodeV4>());
        return bestNode;
    }

    //Newly written closest task 
    private KDNodeV4 GetClosestTask(Vector2 position, KDNodeV4 currentNode, KDNodeV4 bestNode, List<KDNodeV4> checkedNodes)
    {
        //If we haven't already added it as checked, add it
        if(!checkedNodes.Contains(currentNode))
            checkedNodes.Add(currentNode);

        //If a node is a leaf node check if the task is closer
        if(currentNode.task != null)
        {
            //Checking distance based on Astar
            Vector3 convertedPos = position;
            Spot posSpot = gridManager.GetSpot(convertedPos);
            float bestPathDistance = pathFinder.GetPath(posSpot, gridManager.GetSpot(bestNode.task.obj.transform.position)).Count;
            float checkPath = pathFinder.GetPath(posSpot, gridManager.GetSpot(currentNode.task.obj.transform.position)).Count;

            //This is path length is the same check which is actually closer
            if (bestPathDistance == checkPath)
            {
                bestPathDistance = Vector2.Distance(position, bestNode.task.obj.transform.position);
                checkPath = Vector2.Distance(position, currentNode.task.obj.transform.position);
            }

            KDNodeV4 newBestNode = checkPath < bestPathDistance ? currentNode : bestNode;

            //If there is only a root node and no other then return the first task as best 
            if (currentNode.parent == null)
                return newBestNode;
            else
                return GetClosestTask(position, currentNode.parent, newBestNode, checkedNodes);
        }

        int axis = currentNode.axis;

        //if d < 0 outside of cirlce, if d > 0 inside circle, if d == 0 on cirlce edge 
        bool inCircle = false;

        //Checking if our current node needs pruned as it is not closer to the position 
        if (currentNode.parent != null)
        {
            float r = Vector2.Distance(position, bestNode.task.obj.transform.position);
            float d;

            if (axis == x)
            {
                 d = Mathf.Pow(r, 2) - Mathf.Pow(position.x - currentNode.medianPoint.x, 2);
            }
            else
            {
                d = Mathf.Pow(r, 2) - Mathf.Pow(position.y - currentNode.medianPoint.y, 2);
            }

            if (d < 0)
            {
                return GetClosestTask(position, currentNode.parent, bestNode, checkedNodes);
            }
            else
            {
                inCircle = true;
            }

            //not going back up the treee when children have been checked and inside the circle
        }

        //If right child has not be checked
        if(!checkedNodes.Contains(currentNode.rightChild))
        {
            //If both right and left have not been checked 
            if(!checkedNodes.Contains(currentNode.leftChild))
            {
                if (axis == x)
                {
                    if (position.x < currentNode.medianPoint.x)
                        return GetClosestTask(position, currentNode.leftChild, bestNode, checkedNodes);
                    else
                        return GetClosestTask(position, currentNode.rightChild, bestNode, checkedNodes);
                }
                else
                {
                    if (position.y < currentNode.medianPoint.y)
                        return GetClosestTask(position, currentNode.leftChild, bestNode, checkedNodes);
                    else
                        return GetClosestTask(position, currentNode.rightChild, bestNode, checkedNodes);
                }
            }
            //If only the right child has not been checked
            else
            {
                return GetClosestTask(position, currentNode.rightChild, bestNode, checkedNodes);
            }
        }

        //If the left child has not been checked 
        if(!checkedNodes.Contains(currentNode.leftChild))
        {
            return GetClosestTask(position, currentNode.leftChild, bestNode, checkedNodes);
        }

        //If children have been checked and the point is in the circle 
        if (inCircle)
        {
            return GetClosestTask(position, currentNode.parent, bestNode, checkedNodes);
        }
        else
        {
            //If there is no parent and all children have been checked, then the best task is assigned 
            return bestNode;
        }
    }

    //This method takes a node and removes it 
    public void RemoveNode(KDNodeV4 node)
    {
        //Removing task from the tasklist 
        taskList.Remove(node.task);

        //If there is only a root node (1 node only)
        if (node.parent == null)
            return;

        KDNodeV4 parent = node.parent;
        KDNodeV4 oppositeNode = null;

        //Getting the oppsite node to the removal node 
        /*if (parent.axis == x)
            oppositeNode = parent.leftChild.Equals(node) ? parent.leftChild : parent.rightChild;
        else
            oppositeNode = node.medianPoint.y < parent.medianPoint.y ? parent.rightChild : parent.leftChild;*/

        oppositeNode = parent.leftChild.Equals(node) ? parent.rightChild : parent.leftChild;

        oppositeNode.axis = parent.axis;

        //If there is only a depth of 2
        if (parent.parent == null)
        {
            oppositeNode.parent = null;
            rootNode = oppositeNode;
            return;
        }

        //Getting the grandparent node and setting the new parent to the rotated node
        KDNodeV4 grandParent = parent.parent;
        oppositeNode.parent = grandParent;

        //If there is more than a depth of 2
        //Find which child of the grandparent is the parent and replacing it with the opposite
        if (grandParent.leftChild.Equals(parent))
            grandParent.leftChild = oppositeNode;
        else
            grandParent.rightChild = oppositeNode;
    }

    #endregion

    #region Debug Method

    //if the tree needs printed use this 
    private string PrintTree(KDNodeV4 node)
    {
        if(node.leftChild != null)
        {
            Debug.Log(node.medianPoint + " left child: " + PrintTree(node.leftChild));
        }
        if(node.rightChild != null)
        {
            Debug.Log(node.medianPoint + " right child: " + PrintTree(node.rightChild));
        }

        return node.medianPoint.ToString();
    }

    #endregion
}