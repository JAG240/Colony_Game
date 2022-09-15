using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string Name { get; set; }
    public Dictionary<ObjectTaskScript.TaskType, float> Skills = new Dictionary<ObjectTaskScript.TaskType, float>();
    public Dictionary<string, float> Attributes = new Dictionary<string, float>();
    public Character(string name, float chop, float mine, float build, float carryWeight)
    {
        this.Name = name;
        Skills.Add(ObjectTaskScript.TaskType.chop, chop);
        Skills.Add(ObjectTaskScript.TaskType.mine, mine);
        Skills.Add(ObjectTaskScript.TaskType.build, build);
        Attributes.Add("carryWeight", carryWeight);
    }

    public Character()
    {
        this.Name = "Default";
        Skills.Add(ObjectTaskScript.TaskType.chop, 1f);
        Skills.Add(ObjectTaskScript.TaskType.mine, 1f);
        Skills.Add(ObjectTaskScript.TaskType.build, 1f);
        Attributes.Add("carryWeight", 1f);
    }
}
