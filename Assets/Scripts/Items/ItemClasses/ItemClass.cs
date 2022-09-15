using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : MonoBehaviour
{
    public string Name { get; set; }
    public int ID { get; set; }
    public float weight { get; set; }

    public ItemClass()
    {
        this.Name = "";
        this.ID = 0;
        this.weight = 0f;
    }

    public ItemClass(string Name, int ID, float weight)
    {
        this.Name = Name;
        this.ID = ID;
        this.weight = weight;
    }
}
