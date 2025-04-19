using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    [SerializeField]
    
    public GameObject door;
    public GameObject doorbot;
    public GameObject doortop;
    public GameObject plate;
    public GameObject platebot;
    public GameObject platetop;
    public float doorspeed;
    public float platespeed;

    bool IsPressed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        IsPressed = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        IsPressed = false;
    }

    


    private void Update() {
        if (IsPressed) 
        {
            plate.transform.position = Vector2.MoveTowards(plate.transform.position, platebot.transform.position, platespeed);
            door.transform.position = Vector2.MoveTowards(door.transform.position, doortop.transform.position, doorspeed);
        }
        else if (!IsPressed)
        {
            plate.transform.position = Vector2.MoveTowards(plate.transform.position, platetop.transform.position, platespeed);
            door.transform.position = Vector2.MoveTowards(door.transform.position, doorbot.transform.position, doorspeed);
        }
    }
}
