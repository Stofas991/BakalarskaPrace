using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WFCTile))]
public class WFCTileEditor : Editor
{

    public override void OnInspectorGUI()
    {
        WFCTile tileWeight = (WFCTile)target;

        DrawDefaultInspector();

        tileWeight.typeAndWeight.type = tileWeight.tileType;
        tileWeight.typeAndWeight.weight = CalculateWeight(tileWeight.tileType);
        tileWeight.SetValues();

        serializedObject.ApplyModifiedProperties();

    }
    public int CalculateWeight(TileType type)
    {
        switch (type)
        {
            case TileType.Grass:
                return GameConstants.grassWeight;
            case TileType.Trees:
                return GameConstants.treesWeight;
            case TileType.Water:
                return GameConstants.waterWeight;
            case TileType.Beach:
                return GameConstants.beachWeight;
            default:
                return 0;
        }
    }
}

public static class GameConstants
{
    public const int grassWeight = 32;
    public const int treesWeight = 1;
    public const int waterWeight = 5;
    public const int beachWeight = 1;
}
