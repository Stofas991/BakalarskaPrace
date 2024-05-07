/*
 * File: BuildingHUD.cs
 * Author: Kryštof Glos
 * Date: 27.3.2024
 * Description: Manages the building HUD, including categories and items.
 */
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildingHUD : Singleton<BuildingHUD>
{
    [SerializeField] List<UICategory> categories;
    [SerializeField] Transform wrapperElement;
    [SerializeField] GameObject categoryPrefab;
    [SerializeField] GameObject itemPrefab;

    Dictionary<UICategory, GameObject> uiElements = new Dictionary<UICategory, GameObject>();
    Dictionary<GameObject, Transform> elementItemSlot = new Dictionary<GameObject, Transform>();

    IngameMenu ingameMenu;

    private void Start()
    {
        ingameMenu = IngameMenu.GetInstance();
        BuildUI();
    }

    private void BuildUI()
    {
        //loop through categories and creation of buttons automatically
        foreach (UICategory category in categories)
        {
            if (!uiElements.ContainsKey(category))
            {
                var instance = Instantiate(categoryPrefab, Vector3.zero, Quaternion.identity);
                instance.transform.SetParent(wrapperElement, false);

                uiElements[category] = instance;

                //renaming submenu
                var subMenu = instance.transform.Find("CategorySubMenu");
                subMenu.transform.name = category.name + "SubMenu";

                //adding onClick event for button to show menu
                Button button = instance.GetComponent<Button>();
                button.onClick.AddListener(delegate { ingameMenu.ActivateSubMenu(subMenu.gameObject); });

                elementItemSlot[instance] = subMenu;
            }

            uiElements[category].name = category.name;

            TMP_Text text = uiElements[category].GetComponentInChildren<TMP_Text>();
            text.text = category.name;

            uiElements[category].transform.SetSiblingIndex(category.SiblingIndex);
        }

        BuildableObjectBase[] buildables = GetAllBuildables();

        //loop for adding buildable items
        foreach (var buildable in buildables)
        {
            if (buildable == null)
                continue;

            var itemsParent = elementItemSlot[uiElements[buildable.UICategory]];

            var instance = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            instance.transform.SetParent(itemsParent, false);

            instance.name = buildable.name;

            Image img = instance.GetComponentsInChildren<Image>()[1];

            if (buildable.TileBase.GetType() == typeof(RuleTile))
            {
                RuleTile t = (RuleTile)buildable.TileBase;
                img.sprite = t.m_DefaultSprite;
            }
            else
            {
                Tile t = (Tile)buildable.TileBase;
                img.sprite = t.sprite;
            }

            TMP_Text[] texts = instance.GetComponentsInChildren<TMP_Text>();
            texts[0].text = buildable.name;
            texts[1].text = buildable.RequiredResources.ammount.ToString();


            var script = instance.GetComponent<BuildingButtonHandler>();
            script.Item = buildable;
        }
    }

    //Gets all buildableobjectbases from Resources/ScriptableObjects/Buildables
    private BuildableObjectBase[] GetAllBuildables()
    {
        return Resources.LoadAll<BuildableObjectBase>("ScriptableObjects/Buildables");
    }
}
