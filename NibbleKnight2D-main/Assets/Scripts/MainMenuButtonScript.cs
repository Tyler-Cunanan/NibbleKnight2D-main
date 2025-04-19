using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour
{

    public GameObject m_MainMenuButtonScript;
    public GameObject m_Options;

    public GameObject m_ExitScreen;

    public GameObject m_ContinueButton;

    public AudioSource m_BackgroundMusicAudioSource;
    private bool canExit;

    void Start()
    {
        if(PlayerPrefs.HasKey("musicVolume"))
        {
            m_BackgroundMusicAudioSource.volume = PlayerPrefs.GetFloat("musicVolume");
        }
        if(PlayerPrefs.HasKey("SaveKey"))
        {
            m_ContinueButton.SetActive(true);
        }
        canExit = false;
    }

    public void StartGame()
    {
        // Debug.Log("Game Starting");
        SceneManager.LoadScene("MainGame");
    }

    public void Options()
    {
        // Debug.Log("Options Pressed");
        PlayerPrefs.SetFloat("prevMusicVolume", m_BackgroundMusicAudioSource.volume);
        m_MainMenuButtonScript.SetActive(false);
        m_Options.SetActive(true);
    }

    public void LoadGame()
    {
        if(PlayerPrefs.HasKey("SaveKey"))
        {
            // Debug.Log("Loading...");
            PlayerPrefs.SetInt("LoadGame", 1);
            SceneManager.LoadScene("MainGame");
        }
    }

    public void Credits()
    {
        // Debug.Log("Credits Pressed");
        SceneManager.LoadScene("CreditsScene");
    }

    public void Exit()
    {
        // Debug.Log("Exit Pressed");
        m_MainMenuButtonScript.SetActive(false);
        m_ExitScreen.SetActive(true);
        canExit = true;
    }

    void Update()
    {
        if (canExit && Input.GetKeyDown(KeyCode.Return))
        {
            Application.Quit();
        }
    }
}
