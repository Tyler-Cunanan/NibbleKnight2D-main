using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject m_PauseMenuUI;
    public GameObject m_Options;
    public GameObject m_PauseMenuButtons;
    public AudioSource m_BackgroundMusicAudioSource;


    private bool m_isPaused = false;

    // Might Use Later
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(m_isPaused)
            {
                Resume();
            }
            else 
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Pause();
            }
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        m_PauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        m_isPaused = false;
    }

    void Pause()
    {
        m_PauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        m_isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //TODO: Have to press pause button to move again, check if this happens during alpha build.
    }

    public void Options()
    {
        Debug.Log("Options Pressed");
        PlayerPrefs.SetFloat("prevMusicVolume", m_BackgroundMusicAudioSource.volume);
        m_PauseMenuButtons.SetActive(false);
        m_Options.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Quit()
    {
        Debug.Log("Exit Pressed");
        Application.Quit();
    }
}
