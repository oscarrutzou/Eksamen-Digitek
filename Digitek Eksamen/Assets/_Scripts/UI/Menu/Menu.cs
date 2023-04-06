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

    [Header("Pause Menu")]
    public bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    [Header("Level")]
    [SerializeField] public int levelNumber;

    [Header("InkJSON Filer til baner")]
    public TextAsset inkJSON_KlodsHans;

    [Header("Music")]
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private Slider musiclVolumeSlider;
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] public AudioMixerGroup dialogueMixer;
    [SerializeField] private Slider dialogueVolumeSlider;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("musicVolume") && !PlayerPrefs.HasKey("sfxVolume") && !PlayerPrefs.HasKey("dialogueVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.SetFloat("sfxVolume", 1);
            PlayerPrefs.SetFloat("dialogueVolume", 1);
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
        //Dramatisk g�r fra -60DB til -40DB, dog kan mennesket ikke h�rer under -40DB s� det g�r:)
        musicMixer.audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        sfxMixer.audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        dialogueMixer.audioMixer.SetFloat("dialogueVolume", Mathf.Log10(volume) * 20);
        Debug.Log("PlayerPrefs.GetFloat(dialogueVolume)" + PlayerPrefs.GetFloat("dialogueVolume"));
    }

    public void ChangeMusicVolume()
    {
        //S�tter volume til valuen p� slideren og laver en save.
        AudioListener.volume = musiclVolumeSlider.value;
        Save();
    }
    public void ChangeSFXVolume()
    {
        //S�tter volume til valuen p� slideren og laver en save.
        AudioListener.volume = sfxVolumeSlider.value;
        Save();
    }
    public void ChangeDialogueVolume()
    {
        //S�tter volume til valuen p� slideren og laver en save.
        AudioListener.volume = dialogueVolumeSlider.value;
        Save();
    }

    private void Load()
    {
        //S�tter den lig med hvad vi har gemt
        musiclVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        dialogueVolumeSlider.value = PlayerPrefs.GetFloat("dialogueVolume");
    }

    private void Save()
    {
        //S�tter value fra slider ind i vores key name
        PlayerPrefs.SetFloat("musicVolume", musiclVolumeSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("dialogueVolume", dialogueVolumeSlider.value);
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

    public void StartIntroDialogue(TextAsset inkJSON)
    {

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

        //Den skal ikke v�re hardkodet.
        SceneManager.LoadScene(0);
    }
    #endregion

}
