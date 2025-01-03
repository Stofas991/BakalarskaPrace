/*
 * File: InteractButtonHandler.cs
 * Author: Kry�tof Glos
 * Date: 25.4.2024
 * Description: Manages interaction with items.
 */
using UnityEngine;
using UnityEngine.UI;

public class InteractButtonHandler : MonoBehaviour
{
    Button button;
    ItemInteract itemInteract;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        itemInteract = GetComponentInParent<ItemInteract>();
        button.onClick.AddListener(ButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void ButtonClicked()
    {
        itemInteract.DestroyInstanceAndCarry();
    }
}
