using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{

    public AudioSource m_BackgroundMusic;

    GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        m_BackgroundMusic.volume = PlayerPrefs.GetFloat("musicVolume");
        if(Time.timeScale == 0.0f)
        {
            Time.timeScale = 1.0f;
        }

        if(PlayerPrefs.HasKey("LoadGame"))
        {
            /*TODO: Modify and remove GameObject.Find, time extensive search

            Also, use PlayerPrefs.GetString("SaveKey") to load data from
            */
            player.transform.position = GameObject.Find("SavePoint").transform.position;
            PlayerPrefs.DeleteKey("LoadGame");
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
