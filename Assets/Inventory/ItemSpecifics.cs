using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSpecifics : MonoBehaviour
{

    public int count;

    public GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        text.GetComponent<TextMeshPro>().text = count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
