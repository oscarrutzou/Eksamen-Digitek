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

    [Header("Death Menu")]
    public bool playerIsDead = false;
    public GameObject deathMenuUI;

    [Header("Level info")]
    [SerializeField] public int levelNumber;

    public LevelData[] levelData;
    public int currentlevelIntro;

    [Header("Music")]
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] public AudioMixerGroup dialogueMixer;
    [SerializeField] private Slider dialogueVolumeSlider;


    private static Menu instance;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        if (!PlayerPrefs.HasKey("musicVolume") && !PlayerPrefs.HasKey("sfxVolume") && !PlayerPrefs.HasKey("dialogueVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.SetFloat("sfxVolume", 1);
            PlayerPrefs.SetFloat("dialogueVolume", 1);
            Save();
        }
        else
        {
            Load();
        }
    }
    public static Menu GetInstance()
    {
        return instance;
    }

    #region OptionMenu
    public void SetVolume(float volume)
    {
        //Dramatisk går fra -60DB til -40DB, dog kan mennesket ikke hører under -40DB så det går:)
        musicMixer.audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        sfxMixer.audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        dialogueMixer.audioMixer.SetFloat("dialogueVolume", Mathf.Log10(volume) * 20);
        //Debug.Log("PlayerPrefs.GetFloat(dialogueVolume)" + PlayerPrefs.GetFloat("dialogueVolume"));
    }

    public void ChangeMusicVolume()
    {
        //Sætter volume til valuen på slideren og laver en save.
        AudioListener.volume = musicVolumeSlider.value;
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);

    }
    public void ChangeSFXVolume()
    {
        //Sætter volume til valuen på slideren og laver en save.
        AudioListener.volume = sfxVolumeSlider.value;
        PlayerPrefs.SetFloat("sfxVolume", sfxVolumeSlider.value);

    }
    public void ChangeDialogueVolume()
    {
        //Sætter volume til valuen på slideren og laver en save.
        AudioListener.volume = dialogueVolumeSlider.value;
        PlayerPrefs.SetFloat("dialogueVolume", dialogueVolumeSlider.value);

    }

    private void Load()
    {
        //Sætter den lig med hvad vi har gemt
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        dialogueVolumeSlider.value = PlayerPrefs.GetFloat("dialogueVolume");
    }

    private void Save()
    {
        //Sætter value fra slider ind i vores key name
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
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

    #endregion

    #region Pause Menu

    public void PauseMenu()
    {
        //Pause UI menu
        if (gameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    public void Pause()
    {
        //Aktivere objectet
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
        Time.timeScale = 1f;
    }

    public void Resume()
    {
        //Stopper objectet
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            gameIsPaused = false;
            Time.timeScale = 1;
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

    #region Level Selector
    public void LevelSelector(int levelNumber)
    {
        // Check if level number is within the bounds of the array
        if (levelNumber < 1 || levelNumber > levelData.Length)
        {
            Debug.LogError("Level number out of range!");
            return;
        }

        // Get the level at the specified index
        LevelData level = levelData[levelNumber - 1];

        // Check if the level is locked
        if (!level.locked)
        {
            currentlevelIntro = levelNumber;
            // Load the inkJSON for the level
            // Dette er denne som sender en videre efter den er færdig
            DialogueManager.GetInstance().EnterDialogueMode(level.inkJSON);
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(currentlevelIntro);
    }

    #endregion

    public void OnDeathMenu()
    {
        Debug.Log("Hvad der sker når man er død i menu");
    }

    public void TryAgainDeathMenu()
    {
        Debug.Log("Try again");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

[System.Serializable]
public class Level
{
    public int LvlNumber;
    public bool locked;
    public string LvlName;
    public TextAsset inkJSON;
}