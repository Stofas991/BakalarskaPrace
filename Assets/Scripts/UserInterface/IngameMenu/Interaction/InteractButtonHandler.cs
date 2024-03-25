using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
