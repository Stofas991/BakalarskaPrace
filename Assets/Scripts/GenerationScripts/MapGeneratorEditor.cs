using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }
        if (GUILayout.Button("Generate"))
        {
            GenerateFunction(mapGen);
        }
    }
    
    public void GenerateFunction(MapGenerator mapGen)
    {
        if (SelectedValues.isSet)
        {
            mapGen.seed = SelectedValues.seed;
            mapGen.mapHeight = SelectedValues.mapSize;
            mapGen.mapWidth = SelectedValues.mapSize;
        }
        mapGen.GenerateMap();
    }

}