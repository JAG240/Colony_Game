using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldGen))]
public class SaveSeedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WorldGen worldGen = (WorldGen)target;
        if(GUILayout.Button("Save Seed"))
        {
            worldGen.SaveSeed();
        }
    }
}
