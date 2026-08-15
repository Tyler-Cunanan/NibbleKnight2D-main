using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheeseDisplayer : MonoBehaviour
{
    public TextMeshProUGUI cheeseDisplayText;
    public CollectableStore collectableStoreScript;

    public int localCheeseNum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        collectableStoreScript.cheeseAmount = localCheeseNum;
        cheeseDisplayText.text = localCheeseNum.ToString();
    }
    public void AddCheese()
    {
        localCheeseNum++;
    }

    // Reset the number to its default value
    public void ResetNumber()
    {
        localCheeseNum = 0; // Set to whatever your default reset value should be
        Debug.Log("Number reset to: " + localCheeseNum);
    }
}
