using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollisionScript : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("coin picked up");
        Destroy(gameObject);
    }
}
