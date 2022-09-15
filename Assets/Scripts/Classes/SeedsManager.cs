using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SeedsManager : MonoBehaviour
{
    private string path = "E:/Unity Games WIP/Setup/Colony/Assets/SavedSeeds.txt";
    private string[] csvSeperator = new string[] { "\n" };
    private string[] splitSeeds;
    private List<string> seeds = new List<string>();

    private void Awake()
    {
        string seedsDirty = File.ReadAllText(path);
        splitSeeds = seedsDirty.Split(csvSeperator, StringSplitOptions.None);

        for(int i = 0; i < splitSeeds.Length; i++)
        {
            if(splitSeeds[i] != "")
            {
                seeds.Add(splitSeeds[i]);
            }
        }
    }

    public int GetSeed()
    {
        if(seeds.Count == 0)
        {
            return 0;
        }

        return int.Parse(seeds[UnityEngine.Random.Range(0, seeds.Count)]);
    }

    public void SaveSeed(int seed)
    {
        bool exist = false;

        if (seeds.Count == 0)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                seeds.Add(seed.ToString());
                sw.WriteLine(seed);
                Debug.Log(seed + " saved");
            }
        }
        else
        {
            foreach (string s in seeds)
            {
                if (seed == int.Parse(s))
                {
                    Debug.Log("This seed is already saved");
                    exist = true;
                }
            }

            if(!exist)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    seeds.Add(seed.ToString());
                    sw.WriteLine(seed);
                    Debug.Log(seed + " saved");
                }
            }
        }
    }
}
