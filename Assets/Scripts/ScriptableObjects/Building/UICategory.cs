/**
* File: UICategory.cs
* Author: Kryštof Glos
* Date Last Modified: 20.3.2024
* Description: This script defines properties for UI categories used in building interfaces.
*/
using UnityEngine;

[CreateAssetMenu(fileName = "UICategory", menuName = "Building/Create UI Category")]
public class UICategory : ScriptableObject
{
    [SerializeField] int siblingIndex = 0;
    [SerializeField] Color backgroundColor;

    /// <summary>
    /// The sibling index used to determine the order of UI categories.
    /// </summary>
    public int SiblingIndex
    {
        get { return siblingIndex; }
    }

    /// <summary>
    /// The background color associated with the UI category.
    /// </summary>
    public Color BackgroundColor
    {
        get { return backgroundColor; }
    }
}
