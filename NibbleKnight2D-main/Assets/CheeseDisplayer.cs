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
        localCheeseNum = collectableStoreScript.cheeseAmount;
        cheeseDisplayText.text = localCheeseNum.ToString();
    }
}
