using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class Menu : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioMixer globalMixer;
    [SerializeField] Slider globalVolumeSlider;
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] Slider musiclVolumeSlider;
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] Slider sfxVolumeSlider;

    [Header("Pause Menu")]
    public bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    [Header("Level")]
    [SerializeField] public int levelNumber;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("globalVolume") && !PlayerPrefs.HasKey("musicVolume") && !PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("globalVolume", 1);
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.SetFloat("sfxVolume", 1);
            //Load();
        }
        else
        {
            //Load();
        }
    }

    #region OptionMenu
    public void SetVolume(float volume)
    {
        //Dramatisk går fra -60DB til -40DB, dog kan mennesket ikke hører under -40DB så det går:)
        globalMixer.SetFloat("globalVolume", Mathf.Log10(volume) * 20);
        musicMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        sfxMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
    }

    public void ChangeGlobalVolume()
    {
        //Sætter volume til valuen på slideren og laver en save.
        AudioListener.volume = globalVolumeSlider.value;
        Save();
    }
    public void ChangeMusicVolume()
    {
        //Sætter volume til valuen på slideren og laver en save.
        AudioListener.volume = musiclVolumeSlider.value;
        Save();
    }
    public void ChangeSFXVolume()
    {
        //Sætter volume til valuen på slideren og laver en save.
        AudioListener.volume = sfxVolumeSlider.value;
        Save();
    }

    private void Load()
    {
        //Sætter den lig med hvad vi har gemt
        globalVolumeSlider.value = PlayerPrefs.GetFloat("globalVolume");
        musiclVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
    }

    private void Save()
    {
        //Sætter value fra slider ind i vores key name
        PlayerPrefs.SetFloat("globalVolume", globalVolumeSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musiclVolumeSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolumeSlider.value);
    }
    #endregion

    #region MainMenu
    public void PlayGame()
    {
        //Ik hardkode det ind
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    #endregion

    #region Pause Menu
    public void Pause()
    {
        //Aktivere objectet
        pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        //Stopper objectet
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        else
        {
            return;
        }
    }

    public void LoadMenu()
    {
        //Sender brugeren tilbage til den tidligere scene som er menu

        //Den skal ikke være hardkodet.
        SceneManager.LoadScene(0);
    }
    #endregion

}
