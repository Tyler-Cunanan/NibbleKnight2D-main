using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCollisionScript : MonoBehaviour
{
    public CollectableStore collectableStoreScript;

    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("coin picked up");
            collectableStoreScript.AddCheese();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("coin picked up");
            collectableStoreScript.AddCheese();
            Destroy(gameObject);
        }
    }
}
