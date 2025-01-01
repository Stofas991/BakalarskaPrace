using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : Singleton<CharacterUI>
{
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text totalLevel;
    [SerializeField] TMP_Text treeCuttingLevel;
    [SerializeField] TMP_Text miningLevel;
    [SerializeField] TMP_Text fightingLevel;
    public Image characterIcon;

    private void Start()
    {
        HideBox();
    }
    public void ShowBoxForCharacter(UnitStats unit, Sprite characterImage)
    {
        // Show the box with the character's stats
        Debug.Log("ShowBoxForCharacter called for {name}");
        gameObject.SetActive(true);
        characterName.text = unit.characterName;
        characterIcon.sprite = characterImage;
        totalLevel.text = unit.totalLevel.ToString();
        treeCuttingLevel.text = unit.treeCuttingLevel.ToString();
        miningLevel.text = unit.miningLevel.ToString();
        fightingLevel.text = unit.fightingLevel.ToString();
    }

    public void HideBox()
    {
        // Hide the box with the character's stats
        gameObject.SetActive(false);
    }
}
