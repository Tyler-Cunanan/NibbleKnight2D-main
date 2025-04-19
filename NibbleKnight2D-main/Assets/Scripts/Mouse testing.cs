using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mousetesting : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Debug.Log("click");
            Debug.Log(Input.mousePosition);
        }
    }
}
