using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour
{
    public PlayerController PlayerController;
    public PlayerInteract PlayerInteract;
    public Menu Menu;
    public AudioManager AudioManager;

    public bool isInMainMenu { get; set; }
    public bool isInLevel { get; set; }

    public bool playerIsAllowedToMove = true;

    [Header("Timeline")]
    public PlayableDirector director;
    public TimelineAsset[] timelineAssets;

    public bool isPlayingCutscene = false;

    private static GameManager instance;

    private bool hasPLayed1_1 = false;
    private bool hasPLayed1_2 = false;
    private bool hasPLayed1_3 = false;
    private bool hasPLayed1_4 = false;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

    }

    public static GameManager GetInstance()
    {
        return instance;
    }


    private void Update()
    {
        CheckIfSceneChanged();

        //Debug.Log("DialogueManager.GetInstance().GetVariableState(brother1_Done)" + DialogueManager.GetInstance().GetVariableState("brother1_Done"));
        ChangeCutSceneManger();
        //ChangeSceneMangager();
    }

    private bool hasPlayedAwakeMenu = false;
    private bool hasPlayedAwakeLvl = false;
    private bool hasPlayedAwakeCutScene = false;

    public void CheckIfSceneChanged()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 0) //Is in main menu
        {
            isInMainMenu = true;
            isInLevel = false;
            if (hasPlayedAwakeMenu) return;
            FadeMenuInMainMenu();
        }
        else if (currentSceneIndex == 1) //Is in a level or cutscene
        {
            isInMainMenu = false;
            isInLevel = true;
            if (hasPlayedAwakeLvl) return;
            FadeBlackOut();
            hasPlayedAwakeLvl = true;
        }
        else if (currentSceneIndex == 2) //In cutscene
        {
            isInMainMenu = false;
            isInLevel = true;
            if (hasPlayedAwakeCutScene) return;
            StartCutscene1_1();
            hasPlayedAwakeCutScene = true;
        }
    }

    public void OnChangeFromMenuToLevel()
    {
        if (isInMainMenu)
        {
            MainMenuToLvl();
        }
    }


    private void ChangeCutSceneManger()
    {
        //Spiller 1_1 start

        if (((Ink.Runtime.BoolValue)DialogueManager.GetInstance().GetVariableState("brother1_Done")).value == true && !hasPLayed1_2)
        {
            StartCutscene1_2();
        }
        else if (((Ink.Runtime.BoolValue)DialogueManager.GetInstance().GetVariableState("brother2_Done")).value == true && hasPLayed1_2 && !hasPLayed1_3)
        {
            StartCutscene1_3();
        }
        else if (((Ink.Runtime.BoolValue)DialogueManager.GetInstance().GetVariableState("klodsHans_Done")).value == true && hasPLayed1_3 && !hasPLayed1_4)
        {
            StartCutscene1_4();
        }
    }


    #region ChangeMovement
    public void StartMountMovement()
    {
        //Starter special mov, altså ged mov
        PlayerController.MountMovement(true);
    }
    public void StopMountMovement()
    {
        //Stopper special mov, altså ged mov
        PlayerController.MountMovement(false);
    }


    //Vec start af 
    public void StartGridMovement()
    {
        //Start grid mov
        //Pause og spil start cutscene
        PlayerController.GridMovement(true);
    }

    public void StopGridMovement()
    {
        //Stopper grid mov
        PlayerController.GridMovement(false);
    }

    #endregion

    #region FadeInAndOut
    public void FadeBlackIn()
    {
        //0 er fadeIn
        director.playableAsset = timelineAssets[0];
        director.Play();
        Debug.Log("FadeBlackIn");

    }

    public void FadeBlackOut()
    {
        //1 er fadeOUt
        director.playableAsset = timelineAssets[1];
        director.Play();
        Debug.Log("FadeBlackOut");

    }

    public void FadeMenuIn()
    {
        //2 er FadeMenuIn
        director.playableAsset = timelineAssets[2];
        director.Play();
        Debug.Log("FadeMenuIn");

    }

    public void FadeMenuOut()
    {
        //3 er FadeMenuOut
        director.playableAsset = timelineAssets[3];
        director.Play();
        Debug.Log("FadeMenuOut");

    }


    public void FadeMenuOutFadeBlackIn()
    {
        //4 er FadeMenuOutStillBlack
        director.playableAsset = timelineAssets[4];
        director.Play();
        Debug.Log("FadeMenuOutFadeBlackIn");

    }


    public void FadeBalckOutRestartPos()
    {
        //4 er FadeMenuOutStillBlack
        director.playableAsset = timelineAssets[5];
        director.Play();
        Debug.Log("FadeBalckOutRestartPos");

    }

    public void FadeBlackInChangeScene()
    {
        DialogueManager.GetInstance().dialogueVariables.SaveVariables();
        director.playableAsset = timelineAssets[6];
        director.Play();
        Debug.Log("FadeBlackInChangeScene");

    }

    public void MainMenuToLvl()
    {
        director.playableAsset = timelineAssets[7];
        director.Play();
        Debug.Log("MainMenuToLvl");

    }

    public void LoadMainMenu()
    {
        director.playableAsset = timelineAssets[8];
        director.Play();
        Debug.Log("LoadMainMenu");

    }

    public void FullBlackMenuLeftChangeScene()
    {
        director.playableAsset = timelineAssets[9];
        director.Play();
        Debug.Log("FullBlackMenuLeftChangeScene");

    }

    public void FadeMenuInMainMenu()
    {
        hasPlayedAwakeMenu = true;
        director.playableAsset = timelineAssets[10];
        director.Play();
        Debug.Log("FadeMenuInMainMenu");
    }



    public void SignalChangeScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
        //public void ChangeToCutScene1_1()
    //{
    //    FadeBlackIn();
    //    SceneManager.LoadScene(2);
    //}
    #endregion


    #region CutScene
    //Bliver kaldt ved awake
    public void StartCutscene1_1()
    {
        //Fade ind
        //Klods hans kommer ind, første bror kommer ind
        //Start bror 1 snak, dialog enter
        //Stop når han bliver afvist og inden han flytter sig igen
        if (hasPLayed1_1) return;

        Debug.Log("StartCutscene1_1");
        director.playableAsset = timelineAssets[0];
        director.Play();
        hasPLayed1_1 = true;
    }

    private void StartCutscene1_2()
    {
        //Bror 1 snak done
        //Stop submit.
        //Flyt første bror hen til højre. 
        //Flyt anden bror hen til at gøre klar til at snakke.
        //Start anden bror snak, giv adgang til submit

        Debug.Log("StartCutscene1_2");
        director.playableAsset = timelineAssets[1];
        director.Play();
        hasPLayed1_2 = true;

    }


    private void StartCutscene1_3()
    {
        //Bror 2 snak done
        //Stop submit.
        //Flyt bror 2 hen til højre
        //Start klods hans samtale. 
        //Giv adgang til submit

        Debug.Log("StartCutscene1_3");
        director.playableAsset = timelineAssets[2];
        director.Play();
        hasPLayed1_3 = true;

    }

    private void StartCutscene1_4()
    {
        //Klods hans snak done
        //Dialog exit
        //Klods hans går til højre mod prinsessen, vender sig og går op mod hende.
        //Bg farve blir samme som menu, fade
        //Vis Vind menu

        Debug.Log("StartCutscene1_4");
        director.playableAsset = timelineAssets[3];
        director.Play();
        hasPLayed1_4 = true;

    }

    public void StartDialogue(TextAsset inkJSON)
    {
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }

    public void ExitDialogue()
    {
        DialogueManager.GetInstance().ExitDialogueMode();
    }

    #endregion

    public void PlayerDead()
    {
        Menu.OnDeathMenu(); //Klarer menu delen
    }

}

