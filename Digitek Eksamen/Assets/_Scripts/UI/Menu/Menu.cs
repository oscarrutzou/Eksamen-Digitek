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

    [Header("Win Menu")]
    [SerializeField] private GameObject winMenuUI;

    [Header("Level info")]
    public int levelNumber;
    public LevelData[] levelData;
    [SerializeField] private int currentlevelIntro;

    [Header("Collectable info")]
    [SerializeField] private int collectetCollectable;
    [SerializeField] private int maxCollectable;
    [SerializeField] private TextMeshProUGUI[] collectedInfoText;

    [Header("Music")]
    public AudioMixer masterMixer;
    [SerializeField] private Slider[] masterSliders;
    [SerializeField] private Slider[] musicSliders;
    [SerializeField] private Slider[] sfxSliders;
    [SerializeField] private Slider[] dialogueSliders;


    [Header("Transitions")]
    public bool isMenuFadingOut = false;
    public bool isMenuFadingIn = false;


    private static Menu instance;
    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;


    }
    public static Menu GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        LoadSlidersData();
    }

    #region OptionMenu
    private void LoadSlidersData()
    {
        // Load the volume settings from PlayerPrefs or use a default value of 1f
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float dialogueVolume = PlayerPrefs.GetFloat("DialogueVolume", 1f);

        // Set the volume of the mixer groups

        masterMixer.SetFloat("masterMixer", LinearToDecibel(masterVolume));
        masterMixer.SetFloat("musicMixer", LinearToDecibel(musicVolume));
        masterMixer.SetFloat("sfxMixer", LinearToDecibel(sfxVolume));
        masterMixer.SetFloat("dialogueMixer", LinearToDecibel(dialogueVolume));



        // Set the value of the sliders
        foreach (Slider slider in masterSliders)
        {
            slider.value = masterVolume;
        }
        foreach (Slider slider in musicSliders)
        {
            slider.value = musicVolume;
        }
        foreach (Slider slider in sfxSliders)
        {
            slider.value = sfxVolume;
        }
        foreach (Slider slider in dialogueSliders)
        {
            slider.value = dialogueVolume;
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("masterMixer", LinearToDecibel(volume));
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("musicMixer", LinearToDecibel(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("sfxMixer", LinearToDecibel(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetDialogueVolume(float volume)
    {
        masterMixer.SetFloat("dialogueMixer", LinearToDecibel(volume));
        PlayerPrefs.SetFloat("DialogueVolume", volume);
    }

    private float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
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
        if (isMenuFadingIn) return;

        //Aktivere objectet
        menuPanel.SetActive(true);
        pauseMenuUI.SetActive(true);
        GameManager.GetInstance().FadeMenuIn();

        SaveCollectetScore();
        ShowCollectedScore(levelNumber);

        isMenuFadingIn = true;
        PlayerController.GetInstance().isAllowedToMove = false;


    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            if (isMenuFadingOut) return;


            GameManager.GetInstance().FadeMenuOut();
            
            PlayerController.GetInstance().isAllowedToMove = true;
        }
        else
        {
            return;
        }
    }

    //Kaldt i signal tracks

    public void FadeIn()
    {
        gameIsPaused = true;
        isMenuFadingIn = false;
    }


    public void IsFadingOut()
    {
        isMenuFadingOut = true;
    }

    public void FadeMenuOut()
    {
        menuPanel.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
        isMenuFadingOut = false;
    }


    public void LoadMenu()
    {
        //Sender brugeren tilbage til den tidligere scene som er menu

        //Den skal ikke være hardkodet.
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Death Menu
    public void OnDeathMenu()
    {
        FadeMenuOut();
        isMenuFadingIn = false;

        SaveCollectetScore();
        ShowCollectedScore(levelNumber);
        menuPanel.SetActive(true);
        deathMenuUI.SetActive(true);
        GameManager.GetInstance().FadeMenuIn();
    }

    public void TryAgainDeathMenu()
    {
        //StartChangePos & player
        menuPanel.SetActive(true);
        GameManager.GetInstance().FadeMenuOutFadeBlackIn();

        ////Lav alt mørkt og kun start efter måske 1 sec. Måske en count down?
        //menuPanel.SetActive(false);
        //deathMenuUI.SetActive(false);
        //playerObject.transform.position = playerRestartPoint.transform.position;
        //StartCoroutine(WaitRestartPointBeforeMove());
    }


    public void SignalTryAgainRestartPos()
    {
        playerObject.transform.position = playerRestartPoint.transform.position;
        StartCoroutine(WaitRestartPointBeforeMove());
    }


    private IEnumerator WaitRestartPointBeforeMove()
    {
        playerController.Died = false;
        playerController.GridMovement(true);
        playerController.animator.SetBool("isMoving", false);

        yield return new WaitForFixedUpdate();

        playerController.Died = true;
        playerController.GridMovement(true);
        playerController.animator.SetBool("isMoving", false);

        yield return new WaitForSecondsRealtime(1.5f);

        playerController.Died = false;
        playerController.GridMovement(true);

        menuPanel.SetActive(false);
        deathMenuUI.SetActive(false);
    }

    #endregion

    #region Win Menu
    public void OnWinMenu()
    {
        ShowCollectedScore(levelNumber);
        DeleteCollectedScore();
    }

    public void DeleteCollectedScore()
    {
        PlayerPrefs.SetInt("collectedScore" + levelNumber, 0);
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

    #endregion

    #region Collectable
    public void ShowCollectedScore(int lvlNumber)
    {
        foreach (TextMeshProUGUI info in collectedInfoText)
        {

            info.text = PlayerPrefs.GetInt("collectedScore" + lvlNumber).ToString() + " / " + maxCollectable.ToString();
            Debug.Log("info.text" + info.text);
        }

        Debug.Log("SHowCollectedScore"+ PlayerPrefs.GetInt("collectedScore" + lvlNumber));
    }

    public void AddCurrentCollectableScore()
    {
        //LevelData level = levelData[levelNumber - 1];
        Debug.Log("Add");
        //++levelData[levelNumber - 1].lvlMaxCollectable;
        if (collectetCollectable < maxCollectable)
        {
            ++collectetCollectable;
            Debug.Log("Add success");

        }
        else return;
    }

    public void SaveCollectetScore()
    {
        if (!PlayerPrefs.HasKey("collectedScore" + levelNumber)) //Bruger lvl number som går fra 1 og op
        {
            PlayerPrefs.SetInt("collectedScore" + levelNumber, collectetCollectable);
            Debug.Log("SetInt" + PlayerPrefs.GetInt("collectedScore" + levelNumber));

        }
        else
        {
            if (PlayerPrefs.GetInt("collectedScore" + levelNumber) < collectetCollectable)
            {
                PlayerPrefs.SetInt("collectedScore" + levelNumber, collectetCollectable);
                Debug.Log("SetIntBiggerthanSaved" + PlayerPrefs.GetInt("collectedScore" + levelNumber));

            }
            else 
            {
                //PlayerPrefs.SetInt("collectedScore" + levelNumber, collectetCollectable);
                Debug.Log("GetIntscore" + PlayerPrefs.GetInt("collectedScore" + levelNumber));

                return; 
            }
        }
    }

    //public void DeleteHighScoreOnLvl()
    //{
    //    for (int i = 1; i <= levelData.Length; i++)
    //    {
    //        PlayerPrefs.DeleteKey("collectedScore" + i);
    //    }
    //}
    #endregion
}