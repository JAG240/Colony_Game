using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Grid Manager is going to handle building and updating the spot structure used for the astar pathfinding and some other required functions 
public class GridManager : MonoBehaviour
{
    [SerializeField] public Grid myGrid;
    public Tilemap ground;
    public Tilemap stones;
    public Tilemap walkable;
    [SerializeField] CharacterMovement characterMove;
    [SerializeField] GameObject testing;
    BoundsInt groundBound;
    Dictionary<string, Spot> spots = new Dictionary<string, Spot>();
    Dictionary<string, DecorSpot> decorSpots = new Dictionary<string, DecorSpot>();
    private GameObject[] characters;

    void Start()
    {
        groundBound = ground.cellBounds;
        InitSpots();
        characters = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {

    }

    //Builds all spots and adds them to a master list 
    private void InitSpots()
    {
        for (int x = groundBound.xMin; x < groundBound.xMax; x++)
        {
            for (int y = groundBound.yMin; y < groundBound.yMax; y++)
            {
                //if the spot is walkable, put in spots dictionary 
                if (!stones.ContainsTile(stones.GetTile(stones.WorldToCell(new Vector3Int(x, y, 0)))))
                {
                    Spot tempSpot = new Spot(x, y);
                    spots.Add(tempSpot.name, tempSpot);
                }
                //if a decor spot put in decorspots dictionary 
                else if(stones.ContainsTile(stones.GetTile(stones.WorldToCell(new Vector3Int(x, y, 0)))))
                {
                    DecorSpot tempSpot = new DecorSpot(x, y);
                    decorSpots.Add(tempSpot.name, tempSpot);
                }
            }
        }

        //Builds all spots' neighbors 
        foreach (KeyValuePair<string, Spot> spot in spots)
        {
            SetNeigbors(spot.Value);
        }

        foreach(KeyValuePair<string, DecorSpot> decorSpot in decorSpots)
        {
            BuildAdjCode(decorSpot.Value);
            SetTexture(decorSpot.Value);
        }
    }

    //Updates the surrounding spot and removes the spot passed to the method
    public void RemoveSpot(Spot spot)
    {
        List<Spot> evalSpots = new List<Spot>();

        //Adds all neighbors to spot list and clears adjSpots to be re-configured 
        foreach(Spot evalSpot in spot.adjSpots)
        {
            evalSpots.Add(evalSpot);
            evalSpot.adjSpots.Clear();
        }
        
        //removes the spot from the master list 
        spots.Remove(spot.name);

        //re-builds the neighbors of all spots previsouly connected to spot 
        foreach(Spot evalSpot in evalSpots)
        {
            SetNeigbors(evalSpot);
        }
    }

    //Set the neighbors of a given spot 
    private void SetNeigbors(Spot spot)
    {
        bool[] xCheck = new bool[2];
        bool[] yCheck = new bool[2];

        int counterX = 0;
        int counterY = 0;

        for (int x = spot.x - 1; x < spot.x + 2; x += 2)
        {
            Spot tempspot;
            spots.TryGetValue(x + "," + spot.y, out tempspot);
            if (tempspot != null)
            {
                spot.adjSpots.Add(tempspot);
                xCheck[counterX] = true;
            }
            counterX++;
        }

        for (int y = spot.y - 1; y < spot.y + 2; y += 2)
        {
            Spot tempspot;
            spots.TryGetValue(spot.x + "," + y, out tempspot);
            if (tempspot != null)
            {
                spot.adjSpots.Add(tempspot);
                yCheck[counterY] = true;
            }
            counterY++;
        }


        //Checks x - 1 and y - 1
        if (xCheck[0] && yCheck[0])
        {
            Spot tempspot;
            spots.TryGetValue((spot.x - 1) + "," + (spot.y - 1), out tempspot);
            if (tempspot != null)
            {
                spot.adjSpots.Add(tempspot);
            }
        }

        //Checks x + 1 and y + 1
        if (xCheck[1] && yCheck[1])
        {
            Spot tempspot;
            spots.TryGetValue((spot.x + 1) + "," + (spot.y + 1), out tempspot);
            if (tempspot != null)
            {
                spot.adjSpots.Add(tempspot);
            }
        }

        //Checks x - 1 and y + 1
        if (xCheck[0] && yCheck[1])
        {
            Spot tempspot;
            spots.TryGetValue((spot.x - 1) + "," + (spot.y + 1), out tempspot);
            if (tempspot != null)
            {
                spot.adjSpots.Add(tempspot);
            }
        }

        //Checks x + and y - 1
        if (xCheck[1] && yCheck[0])
        {
            Spot tempspot;
            spots.TryGetValue((spot.x + 1) + "," + (spot.y - 1), out tempspot);
            if (tempspot != null)
            {
                spot.adjSpots.Add(tempspot);
            }
        }
    }

    //Builds the DecorSpots adjCode. Reminder this starts top left and build to top right with 8 combinations 
    private void BuildAdjCode(DecorSpot decorSpot)
    {
        //intialize the code 
        string code = "";
        int[] codeArray = new int[8];
        int counter = 0;

        //loop from top left to bottom right 
        for (int x = decorSpot.x - 1; x < decorSpot.x + 2; x++)
        {
            for (int y = decorSpot.y + 1; y > decorSpot.y - 2; y--)
            {
                //if we are on the center do not concat 
                if (x == decorSpot.x && y == decorSpot.y)
                    continue;

                //checking if a neighbor exists 
                DecorSpot tempSpot;
                decorSpots.TryGetValue(x + "," + y, out tempSpot);

                //if it does concat a 1 otherwise concat a 0
                if (tempSpot != null)
                    codeArray[counter] = 1;
                else
                    codeArray[counter] = 0;

                counter++;
            }
        }

        //Checking top left corner
        if (!(codeArray[1] == 1 && codeArray[3] == 1))
            codeArray[0] = 0;

        //checking bottom left corner
        if (!(codeArray[1] == 1 && codeArray[4] == 1))
            codeArray[2] = 0;

        //checking top right corner
        if (!(codeArray[3] == 1 && codeArray[6] == 1))
            codeArray[5] = 0;

        //checking bottom right corner
        if (!(codeArray[4] == 1 && codeArray[6] == 1))
            codeArray[7] = 0;

        //consructing string
        foreach(int i in codeArray)
        {
            code += i;
        }

        //Set the spot's code
        decorSpot.adjSpotsCode = code;
    }

    //Set a tiles texture correctly based on code 
    public void SetTexture(DecorSpot decorSpot)
    {
        TileBase tileBase = null;
        if((TileBase)Resources.Load("TP Stone Ground/" + decorSpot.adjSpotsCode))
        {
            tileBase = (TileBase)Resources.Load("TP Stone Ground/" + decorSpot.adjSpotsCode);
        }
        else
        {
            Debug.Log(decorSpot.adjSpotsCode.ToString());
            tileBase = (TileBase)Resources.Load("TP Stone Ground/00000000");
        }
        stones.SetTile(new Vector3Int(decorSpot.x, decorSpot.y, 0), tileBase);
    }

    //Converts a vector3 into a spot from the master list 
    public Spot GetSpot(Vector3 position)
    {
        position = ground.WorldToCell(position);
        Spot tempSpot;
        spots.TryGetValue(position.x + "," + position.y, out tempSpot);
        return tempSpot;
    }

    //Shows a red square on all walkable tiles 
    private void DebugWalkable()
    {
        foreach(KeyValuePair<string, Spot> spot in spots)
        {
            Instantiate(testing, new Vector3(spot.Value.x, spot.Value.y, 0), Quaternion.identity);
        }
    }

    //Updates the spots with the charater's tasks scripts
    public void UpdateCharacterSpots()
    {
        foreach(GameObject character in characters)
        {
            GetSpot(character.transform.position).characterTasks = character.GetComponent<CharacterTasks>();
        }
    }

    //Clears all spots from character refernces 
    public void ClearCharacterSpots()
    {
        foreach(KeyValuePair<string,Spot> spot in spots)
        {
            spot.Value.characterTasks = null;
        }
    }

    //Does the function of clearing all spots and updating spots' character location 
    public void RefreshSpots()
    {
        foreach (KeyValuePair<string, Spot> spot in spots)
        {
            spot.Value.characterTasks = null;
        }

        foreach (GameObject character in characters)
        {
            GetSpot(character.transform.position).characterTasks = character.GetComponent<CharacterTasks>();
        }
    }
}