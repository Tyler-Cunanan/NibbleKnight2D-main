using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveAndLoadScript : MonoBehaviour
{

    // public GameObject[] m_SavePoints;

    public GameObject m_Text;

    private bool onCollider;

    private bool actionDone;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && onCollider && !actionDone)
        {
            SaveGame();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && onCollider && !actionDone)
        {
            PlayerPrefs.DeleteKey("SaveKey");
            Debug.Log("2 Pressed, Key is deleted!");
            m_Text.GetComponent<TMP_Text>().text = "Save Data Deleted!";
            actionDone = true;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && onCollider)
        {
            Debug.Log(PlayerPrefs.HasKey("SaveKey"));
        }
    }

    public void SaveGame()
    {
        Debug.Log("1 pressed, you have saved!");
        /*TODO: Add specific data to String:
        name: Swiss
        Date save -> TimeDate.today
        location of save point
        health %
        */
        PlayerPrefs.SetString("SaveKey","1");
        m_Text.GetComponent<TMP_Text>().text = "Data Saved";
        actionDone = true;
    }

    // public void CreatingDataString()
    // {

    // }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision Entered");
        if(other.gameObject.CompareTag("Player"))
        {
            m_Text.SetActive(true);
            m_Text.GetComponent<TMP_Text>().text = "Press 1 to enter save\nPress 2 to delete save\nPress 3 to check status of save";
            onCollider = true;
            actionDone = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        onCollider = false;
        if(m_Text != null && m_Text.activeInHierarchy)
        {
            m_Text.SetActive(false);
            actionDone = false;
        }
    }
}
