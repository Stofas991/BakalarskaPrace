using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterUI : Singleton<CharacterUI>
{
    [SerializeField] TMP_Text characterName;

    private void Start()
    {
        HideBox();
    }
    public void ShowBoxForCharacter(UnitStats unit)
    {
        // Show the box with the character's stats
        Debug.Log("ShowBoxForCharacter called for {name}");
        gameObject.SetActive(true);
        characterName.text = unit.characterName;
    }

    public void HideBox()
    {
        // Hide the box with the character's stats
        gameObject.SetActive(false);
    }
}
