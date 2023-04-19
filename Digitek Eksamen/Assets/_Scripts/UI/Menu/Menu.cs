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
    [Header("Player")]
    public GameObject playerObject;
    public PlayerController playerController;

    [Header("Menu Panel")]
    [SerializeField] private GameObject menuPanel; //Brugt ved både pause og death menu

    [Header("Pause Menu")]
    public bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    [Header("Death Menu")]
    [SerializeField] private GameObject deathMenuUI;
    [SerializeField] private GameObject changePlayerMovStartObject;
    [SerializeField] private Transform playerRestartPoint;
    //private bool deathMenuActive = false;

    [Header("Level info")]
    public int levelNumber;
    public LevelData[] levelData;
    [SerializeField] private int currentlevelIntro;

    [Header("Collectable info")]
    [SerializeField] private int collectetCollectable;
    [SerializeField] private int maxCollectable;

    [Header("Music")]
    [SerializeField] private AudioMixerGroup masterMixer;
    [SerializeField] private Slider masterVolumeSlider;
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

        if (!PlayerPrefs.HasKey("masterVolume")
            && !PlayerPrefs.HasKey("musicVolume") 
            && !PlayerPrefs.HasKey("sfxVolume") 
            && !PlayerPrefs.HasKey("dialogueVolume"))
        {
            PlayerPrefs.SetFloat("masterVolume", 1);
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
        masterMixer.audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
        musicMixer.audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        sfxMixer.audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        dialogueMixer.audioMixer.SetFloat("dialogueVolume", Mathf.Log10(volume) * 20);
    }

    public void ChangeMasterVolume()
    {
        AudioListener.volume = musicVolumeSlider.value;
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
    }

    public void ChangeMusicVolume()
    {
        //AudioListener.volume = musicVolumeSlider.value;
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
        
    }
    public void ChangeSFXVolume()
    {
        //AudioListener.volume = sfxVolumeSlider.value;
        PlayerPrefs.SetFloat("sfxVolume", sfxVolumeSlider.value);
    }

    public void ChangeDialogueVolume()
    {
        //AudioListener.volume = dialogueVolumeSlider.value;
        PlayerPrefs.SetFloat("dialogueVolume", dialogueVolumeSlider.value);

    }

    private void Load()
    {
        //Sætter den lig med hvad vi har gemt
        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");

        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        dialogueVolumeSlider.value = PlayerPrefs.GetFloat("dialogueVolume");
    }

    private void Save()
    {
        //Sætter value fra slider ind i vores key name
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
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
        menuPanel.SetActive(true);
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
        PlayerController.GetInstance().isAllowedToMove = false;
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            menuPanel.SetActive(false);
            pauseMenuUI.SetActive(false);
            gameIsPaused = false;
            PlayerController.GetInstance().isAllowedToMove = true;
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

    #region Collectable
    public void AddCurrentCollectableScore()
    {
        //LevelData level = levelData[levelNumber - 1];

        //++levelData[levelNumber - 1].lvlMaxCollectable;
        if (collectetCollectable < maxCollectable)
        {
            ++collectetCollectable;
        }
        else return;

        Debug.Log("collectetCollectable" + collectetCollectable);

    }

    public void SaveCollectetScore()
    {
        if (!PlayerPrefs.HasKey("collectedScore" + levelNumber)) //Bruger lvl number som går fra 1 og op
        {
            PlayerPrefs.SetInt("collectedScore" + levelNumber, collectetCollectable);
            Debug.Log("Dont have key PlayerPrefs.GetInt(collectedScore + levelNumber): " + PlayerPrefs.GetInt("collectedScore" + levelNumber));

        }
        else
        {
            if (PlayerPrefs.GetInt("collectedScore" + levelNumber) < collectetCollectable)
            {
                PlayerPrefs.SetInt("collectedScore" + levelNumber, collectetCollectable);
                Debug.Log("HasKey PlayerPrefs.GetInt(collectedScore + levelNumber): " + PlayerPrefs.GetInt("collectedScore" + levelNumber));

            }
            else 
            {
                Debug.Log("HasKey123 PlayerPrefs.GetInt(collectedScore + levelNumber): " + PlayerPrefs.GetInt("collectedScore" + levelNumber));
                PlayerPrefs.SetInt("collectedScore" + levelNumber, collectetCollectable);
                return; 
            }
        }
    }

    public void DeleteHighScoreOnLvl()
    {
        for (int i = 1; i <= levelData.Length; i++)
        {
            PlayerPrefs.DeleteKey("collectedScore" + i);
        }
    }

    public void GoThoughShowScore()
    {
        for (int i = 1; i <= levelData.Length; i++)
        {
            ShowMaxCollectetScore(i);
        }
    }

    //Kald i menu
    public void ShowMaxCollectetScore(int maxCollectedInLvl)
    {
        Debug.Log(PlayerPrefs.GetInt("collectedScore" + maxCollectedInLvl));

        PlayerPrefs.GetInt("collectedScore" + maxCollectedInLvl);
    }

    #endregion

    public void OnDeathMenu()
    {
        menuPanel.SetActive(true);
        //deathMenuActive = true;
        deathMenuUI.SetActive(true);
    }

    public void TryAgainDeathMenu()
    {
        //Lav alt mørkt og kun start efter måske 1 sec. Måske en count down?
        menuPanel.SetActive(false);
        //deathMenuActive = false;
        deathMenuUI.SetActive(false);
        playerObject.transform.position = playerRestartPoint.transform.position;
        playerController.Died = false;
        playerController.GridMovement(true);
    }

}