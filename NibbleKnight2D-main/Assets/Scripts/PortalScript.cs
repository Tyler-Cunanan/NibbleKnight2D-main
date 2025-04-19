using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public bool launch;
    public float launch_Velocity = 20f;
    public Transform destination;
    GameObject player;

    public GameObject m_Text;

    private bool onCollider;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        onCollider = false;
    }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Player"))
//         {
//             if (Vector2.Distance(player.transform.position, transform.position) > 0.7f)
//             {
//                 player.transform.position = destination.transform.position;
//             }
//         }
//     }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) && onCollider)
        {
            // if(!launch) {
            //     if (Vector2.Distance(player.transform.position, transform.position) > 0.7f)
            //     {
            //         player.transform.position = destination.transform.position;
            //         // player.velocity = new Vector2(player.velocity.x ,launch_Velocity);
            //         // have the player launch out of the pipe at a high speed
            //     }
            // } else {
            //     if(Input.GetKeyDown(KeyCode.E)) {
            //         if (Vector2.Distance(player.transform.position, transform.position) > 0.7f)
            //         {
            //             player.transform.position = destination.transform.position;
            //         }
            //     }
            // }
            Debug.Log("W pressed");
            if(gameObject.tag == "PortalStandingPipe")
            {
                player.transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y + 2, destination.transform.position.z);  
            }
            else if (gameObject.tag == "PortalWallDoorPipe")
            {
                player.transform.position = destination.transform.position;
            }
            else 
            {
                Debug.Log("Error: Portal Type doesn't exist! Please create tag and modify code as needed to work.");
            }
        }
    } 

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision Entered");
        if(other.gameObject.CompareTag("Player"))
        {
            m_Text.SetActive(true);
            onCollider = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        onCollider = false;
        if(m_Text != null && m_Text.activeInHierarchy)
        {
            m_Text.SetActive(false);
        }
    }
}
