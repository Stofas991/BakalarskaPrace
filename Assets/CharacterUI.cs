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
    public GameObject characterIconBox;
    public Button nextButton;
    public Button previousButton;

    private List<UnitStats> selectedUnits = new List<UnitStats>();
    private List<Sprite> characterImagesLocal = new List<Sprite>();
    private int currentIndex = 0;

    private void Start()
    {
        HideBox();
        nextButton.onClick.AddListener(ShowNextCharacter);
        previousButton.onClick.AddListener(ShowPreviousCharacter);
    }

    public void ShowBoxForCharacters(List<UnitStats> units, List<Sprite> characterImages)
    {
        selectedUnits = units;
        currentIndex = 0;
        characterImagesLocal.Clear();
        characterImagesLocal.AddRange(characterImages);
        
        if (units.Count == 1)
        {
            nextButton.interactable = false;
            previousButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
            previousButton.interactable = false;
        }
        ShowCharacter(currentIndex, characterImages);
    }

    private void ShowCharacter(int index, List<Sprite> characterImages)
    {
        if (index < 0 || index >= selectedUnits.Count) return;

        UnitStats unit = selectedUnits[index];
        gameObject.SetActive(true);
        characterName.text = unit.characterName;
        characterIcon.sprite = characterImages[index];
        totalLevel.text = unit.totalLevel.ToString();
        treeCuttingLevel.text = unit.treeCuttingLevel.ToString();
        miningLevel.text = unit.miningLevel.ToString();
        fightingLevel.text = unit.fightingLevel.ToString();
    }

    private void ShowNextCharacter()
    {
        if (currentIndex < selectedUnits.Count - 1)
        {
            currentIndex++;
            ShowCharacter(currentIndex, GetCharacterImages());
            if (currentIndex < selectedUnits.Count - 1)
            {
                nextButton.interactable = true;
            }
            else
            {
                nextButton.interactable = false;
            }
            previousButton.interactable = true;
        }
    }

    private void ShowPreviousCharacter()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCharacter(currentIndex, GetCharacterImages());
            if (currentIndex > 0)
            {
                previousButton.interactable = true;
            }
            else
            {
                previousButton.interactable = false;
            }
            if (currentIndex < selectedUnits.Count - 1)
            {
                nextButton.interactable = true;
            }
        }
    }

    private List<Sprite> GetCharacterImages()
    {
        return characterImagesLocal;
    }

    public void HideBox()
    {
        gameObject.SetActive(false);
    }
}
