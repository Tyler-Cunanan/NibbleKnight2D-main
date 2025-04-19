using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{

    public GameObject Options;
    public GameObject MainMenuButtonScript;
    public Slider m_MusicSlider;
    public AudioSource m_BackgroundMusicAudioSource;

    void Awake()
    {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.25F);
            m_BackgroundMusicAudioSource.volume = 0.25F;
            
        }
        m_BackgroundMusicAudioSource.volume = PlayerPrefs.GetFloat("musicVolume");
        m_MusicSlider.value = m_BackgroundMusicAudioSource.volume;
    }

    // void Start()
    // {
    //     Load();
    // }

    public void ChangeMusicVolume()
    {
        m_BackgroundMusicAudioSource.volume = m_MusicSlider.value;
        Save();
    }

    // private void Load()
    // {
    //     PlayerPrefs.SetFloat("prevMusicVolume", m_BackgroundMusicAudioSource.volume);
    // }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", m_MusicSlider.value);
    }

    public void ReturnAndSave()
    {
        Options.SetActive(false);
        if(MainMenuButtonScript != null)
        {
            MainMenuButtonScript.SetActive(true);
        }
    }

    public void ReturnWithoutSaving()
    {
        m_BackgroundMusicAudioSource.volume = PlayerPrefs.GetFloat("prevMusicVolume");
        PlayerPrefs.SetFloat("musicVolume", m_BackgroundMusicAudioSource.volume);
        m_MusicSlider.value = m_BackgroundMusicAudioSource.volume;
        Options.SetActive(false);
        if(MainMenuButtonScript != null)
        {
            MainMenuButtonScript.SetActive(true);
        }
    }

}
