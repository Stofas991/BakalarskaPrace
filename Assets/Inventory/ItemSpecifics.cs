using TMPro;
using UnityEngine;

public class ItemSpecifics : MonoBehaviour
{

    public int count;
    public ContainedItemType itemType;

    [SerializeField] GameObject textObject;

    // Start is called before the first frame update
    void Start()
    {
        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = count.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
