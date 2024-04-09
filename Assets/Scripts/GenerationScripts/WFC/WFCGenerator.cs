using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Sentry;

public class WFCGenerator : MonoBehaviour
{
    [SerializeField]GameObject WFCTileObject;
    public bool finished = false;

    public WFCCell PlaceTiles(int posX, int posY, Transform parent)
    {
        var instance = Instantiate(WFCTileObject, new Vector3(posX, posY, 0), new Quaternion());
        instance.transform.parent = parent;
        return instance.GetComponent<WFCCell>();
    }

    public void WaveFunction(WFCCell cellToCollapse)
    {
        cellToCollapse.Collapse();
        
        Stack stack = new Stack();

        stack.Push(cellToCollapse);

        while (stack.Count > 0)
        {
            WFCCell cell = (WFCCell)stack.Pop();
            var directions = cell.GetDirections();

            foreach (var direction in directions)
            {
                List<WFCTile> neighbourAllowedTiles = new List<WFCTile>();
                foreach (WFCTile possibility in cell.GetPossibilities())
                {
                    if (possibility.directionNeighbours.Count == 0)
                        possibility.SetValues();

                    neighbourAllowedTiles.AddRange(possibility.directionNeighbours[direction]);
                }

                neighbourAllowedTiles.Distinct();
                var neighbour = cell.GetNeighbour(direction);
                if (neighbour.entropy != 0)
                {
                    var reduced = neighbour.Constrain(neighbourAllowedTiles, direction);
                    if (reduced)
                        stack.Push(neighbour);
                }
            }
        }
    }

    public List<WFCCell> GetLowestEntropy(Transform parent)
    {
        SentrySdk.CaptureMessage("function lowest entropy");
        List<WFCCell> lowestEntropyCells = new List<WFCCell>();
        int lowestEntropy = 100;

        foreach (Transform child in parent)
        {
            WFCCell currentCell = child.GetComponent<WFCCell>();
            if (currentCell.entropy > 0)
            {
                if (currentCell.entropy < lowestEntropy)
                {
                    lowestEntropyCells.Clear();
                    lowestEntropy = currentCell.entropy;
                }
                if (currentCell.entropy == lowestEntropy)
                {
                    lowestEntropyCells.Add(currentCell);
                }
            }
            if (currentCell.entropy == 0 && !currentCell.collapsed) 
            {
                SentrySdk.CaptureMessage("Cell not collapsed and is zero");
                Debug.Log(currentCell);
            }
        }
        return lowestEntropyCells;
    }
}
