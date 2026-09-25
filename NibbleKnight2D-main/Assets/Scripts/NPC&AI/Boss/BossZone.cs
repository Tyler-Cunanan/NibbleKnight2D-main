using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RobotBossBehaviour;

public class BossZone : MonoBehaviour
{
    public RobotBossBehaviour robotBoss; // Assign this in the inspector

    public GameObject BossStatusDisplay;
    public GameObject Wallup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            // Call FaceTarget with the player's position

            BossStatusDisplay.SetActive(true);
            Wallup.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            // Call FaceTarget with the player's position

        }
    }
}
