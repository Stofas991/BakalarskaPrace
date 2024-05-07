using UnityEditor;

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
            case TileType.Mountain:
                return GameConstants.treesWeight;
            case TileType.Water:
                return GameConstants.waterWeight;
            default:
                return 0;
        }
    }
}

public class GameConstants
{
    static public int grassWeight = 32;
    static public int treesWeight = 1;
    static public int waterWeight = 5;
}
