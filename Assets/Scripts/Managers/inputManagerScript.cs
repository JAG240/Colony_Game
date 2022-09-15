using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class inputManagerScript : MonoBehaviour
{
    #region Camera Settings
    [SerializeField] private float camSpeed = 5f;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float dragThreshold = 0.0f;
    #endregion

    #region Searlized Refs
    [SerializeField] private TaskManagerScript taskManager;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject queueIndicator;
    [SerializeField] private GameObject buildIndicator;
    [SerializeField] private Builder builder;
    #endregion

    #region Vars
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject rock;
    private GameObject activeObject;
    private Vector3 startHold;
    private Vector3 endHold;
    private bool holding = false;
    private bool startHolding = false;
    private ObjectTaskScript.TaskType orderTask;
    public Tilemap stones;
    public Tilemap trees;
    public Tilemap rocks;
    private Tilemap activeMap;
    #endregion

    void Start()
    {

    }

    void Update()
    {
        //camera controls should always be possible 
        CameraControls();

        //if build is not set
        if(orderTask != ObjectTaskScript.TaskType.build)
        {
            //if the build indicator is active make it inactive
            if(buildIndicator.activeInHierarchy)
            {
                buildIndicator.SetActive(false);
            }

            //Do stuff other than building stuff 
            TaskSelect();
        }
        else if(orderTask == ObjectTaskScript.TaskType.build)
        {
            //if we are building and inidcator is not active make it active
            if(!buildIndicator.activeInHierarchy)
            {
                buildIndicator.SetActive(true);
            }

            //if we press the button down make the builder place a building task 
            if(Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                builder.PlacePlannedBuild(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        //Holding variable is used when creating lines for dragging 
        if(holding)
        {
            lineRenderer.SetPosition(0, new Vector3(startHold.x, startHold.y, 0f));
            lineRenderer.SetPosition(1, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, startHold.y, 0f));
            lineRenderer.SetPosition(2, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f));
            lineRenderer.SetPosition(3, new Vector3(startHold.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f));
        }
        else if(!holding)
        {
            for(int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, Vector3.zero);
            }
        }
    }

    //After a task type is selected do stuff 
    private void TaskSelect()
    {
        //if the mouse button is down and there is a task selected
        if (Input.GetMouseButtonDown(0) && orderTask != ObjectTaskScript.TaskType.none)
        {
            //if not over a UI element 
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3Int mousePosClean = activeMap.WorldToCell(Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            //checks if the active map has a tile where clicked 
            if(activeMap.HasTile(mousePosClean))
            {
                //checks if there is already an object there and does not allow duplicate tasks to be made
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePosClean.x, mousePosClean.y), Vector2.zero);
                if(hit)
                {
                    return;
                }

                //creates a task object with no sprite 
                GameObject tempObject = Instantiate(activeObject, mousePosClean, Quaternion.identity);
                Instantiate(queueIndicator, mousePosClean, Quaternion.identity, tempObject.transform);
                tempObject.GetComponent<ObjectTaskScript>().map = activeMap;
                taskManager.AddTask(1, orderTask, tempObject);
            }
        }
        else if(Input.GetMouseButtonDown(0) && orderTask == ObjectTaskScript.TaskType.none)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if(hit && hit.transform.tag == "Player")
            {
                hit.transform.GetComponent<CharacterInventory>().DebugInventory();
            }
            else if(hit && (hit.transform.name == "Chest" || hit.transform.name == "Chest(Clone)" ))
            {
                hit.transform.GetComponent<StorageContainer>().DebugInv();
            }
        }

        //If over UI don't allow more 
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //This code checks if the user is holding rather than clicking. A distance check should really be made 
        if (Input.GetMouseButton(0) && !holding)
        {
            if (!startHolding)
            {
                startHolding = true;
                startHold = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            holding = true;
        }
        else if (holding && !Input.GetMouseButton(0))
        {
            startHolding = false;
            holding = false;
            endHold = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(startHold, endHold) > dragThreshold && orderTask != ObjectTaskScript.TaskType.none)
            {
                Vector2 bottomLeft = new Vector2(Mathf.Min(startHold.x, endHold.x), Mathf.Min(startHold.y, endHold.y));
                Vector2 topRight = new Vector2(Mathf.Max(startHold.x, endHold.x), Mathf.Max(startHold.y, endHold.y));

                GetAllObjBetween(bottomLeft, topRight);
            }
        }
    }

    //Did a raycast between two points
    private void GetAllObjBetween(Vector2 start, Vector2 end)
    {
        for (int x = (int)start.x; x < end.x; x++)
        {
            for (int y = (int)start.y; y < end.y; y++)
            {
                Vector3Int mousePosClean = activeMap.WorldToCell(new Vector3Int(x, y, 0));

                if (activeMap.HasTile(mousePosClean))
                {
                    RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePosClean.x, mousePosClean.y), Vector2.zero);
                    if (hit)
                    {
                        continue;
                    }

                    //creates a task object with no sprite 
                    GameObject tempObject = Instantiate(activeObject, mousePosClean, Quaternion.identity);
                    Instantiate(queueIndicator, mousePosClean, Quaternion.identity, tempObject.transform);
                    tempObject.GetComponent<ObjectTaskScript>().map = activeMap;
                    taskManager.AddTask(1, orderTask, tempObject);
                }
            }
        }
    }

    //Camera controls to control the camera
    private void CameraControls()
    {
        //direct movement from keyboard
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            Camera.main.transform.position += new Vector3(Input.GetAxis("Horizontal") * camSpeed, Input.GetAxis("Vertical") * camSpeed, 0f);
        }

        //zooming in or out with scroll wheel 
        if (Input.mouseScrollDelta.y != 0)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //zoom control contraints 
            if (Camera.main.orthographicSize > 5 && Camera.main.orthographicSize < 25)
            {
                Camera.main.orthographicSize += Input.mouseScrollDelta.y * zoomSpeed * -1;
            }
            else if (Camera.main.orthographicSize <= 5 && Input.mouseScrollDelta.y < 0 || Camera.main.orthographicSize >= 25 && Input.mouseScrollDelta.y > 0)
            {
                Camera.main.orthographicSize += Input.mouseScrollDelta.y * zoomSpeed * -1;
            }
        }
    }

    //Set's order type based on UI button
    public void SetOrderTask(string type)
    {
        switch(type)
        {
            case "Chop":
                orderTask = ObjectTaskScript.TaskType.chop;
                activeMap = trees;
                activeObject = tree;
                break;
            case "Mine":
                orderTask = ObjectTaskScript.TaskType.mine;
                activeMap = rocks;
                activeObject = rock;
                break;
            case "Build":
                orderTask = ObjectTaskScript.TaskType.build;
                activeMap = null;
                activeObject = null;
                break;
            default:
                orderTask = ObjectTaskScript.TaskType.none;
                activeMap = null;
                activeObject = null;
                break;
        }
    }
}
