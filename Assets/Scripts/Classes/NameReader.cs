using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NameReader : MonoBehaviour
{
    private string path = "E:/Unity Games WIP/Setup/Colony/Assets/NamesListFinal.txt";
    private string[] csvSeperator = new string[] { ", " };
    private string[] names;

    void Awake()
    {
        string namesDirty = File.ReadAllText(path);
        names = namesDirty.Split(csvSeperator, StringSplitOptions.None);
    }

    public string GetName()
    {
        return names[UnityEngine.Random.Range(0, names.Length)];
    }
}
