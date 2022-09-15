using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spot
{
    public int x { get; set; }
    public int y { get; set; }
    public string name { get; set; }
    public List<Spot> adjSpots = new List<Spot>();
    public Spot parent { get; set; }
    public int f { get; set; }
    public int g { get; set; }
    public int h { get; set; }
    public task Task { get; set; }
    public CharacterTasks characterTasks{ get; set;}

    public Spot(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.name = x + "," + y;
    }
}
