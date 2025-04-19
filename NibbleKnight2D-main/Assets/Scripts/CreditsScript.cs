using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{

    public AudioSource m_BackgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        m_BackgroundMusic.volume = PlayerPrefs.GetFloat("musicVolume");
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
