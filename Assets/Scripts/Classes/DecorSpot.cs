using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorSpot
{
    public int x { get; set; }
    public int y { get; set; }
    public string name { get; set; }
    public string adjSpotsCode { get; set; }
    public List<DecorSpot> adjSpots = new List<DecorSpot>();
    public task task { get; set; }

    public DecorSpot(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.name = x + "," + y;
        this.adjSpotsCode = "00000000";
    }
}
