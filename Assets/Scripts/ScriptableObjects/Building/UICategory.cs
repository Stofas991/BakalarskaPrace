using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UICategory", menuName = "Building/Create UI Category")]
public class UICategory : ScriptableObject
{
    [SerializeField] int siblingIndex = 0;
    [SerializeField] Color backgroundColor;

    public int SiblingIndex
    {
        get { return siblingIndex; }
    }

    public Color BackgroundColor
    {
        get { return backgroundColor; }
    }
}
