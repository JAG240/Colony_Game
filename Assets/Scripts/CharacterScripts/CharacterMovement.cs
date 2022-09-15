using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minDistance = 1.5f;
    public Vector3 targetPos;
    [SerializeField] private bool moving = false;
    public GridManager gridManager;
    private PathFinder pathFinder = new PathFinder();
    public List<Spot> targetPath = new List<Spot>();

    void Awake()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
    }

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        if(!moving && transform.position != targetPos && targetPath.Count > 0)
        {
            moving = true;
            StartCoroutine(MoveCharacter(targetPath));
        }
    }

    public bool GetMoving()
    {
        return moving;
    }

    public bool GetNearTaskObject()
    {
        if(Vector2.Distance(transform.position, targetPos) <= minDistance)
        {
            return true;
        }

        return false;
    }

    public IEnumerator MoveCharacter(List<Spot> path)
    {
        targetPos = new Vector2(path[path.Count - 1].x, path[path.Count - 1].y);

        foreach(Spot spot in path)
        {
            Vector3 target = new Vector3(spot.x, spot.y, 0);
            while(transform.position != target)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(spot.x, spot.y), speed * Time.deltaTime);
                yield return null;
            }
        }

        targetPath.Clear();
        moving = false;
    }

    public List<Spot> GetPathTo(Vector3 targetPos)
    {
        Spot start, end;
        start = gridManager.GetSpot(transform.position);
        end = gridManager.GetSpot(targetPos);
        if (!(start == null || end == null))
        {
            List<Spot> path = pathFinder.GetPath(start, end);
            if (path != null)
            {
                return path;
            }
        }

        return null;
    }

    public Spot GetCurrentTile()
    {
        return gridManager.GetSpot(transform.position);
    }
}
