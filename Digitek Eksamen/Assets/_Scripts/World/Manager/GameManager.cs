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

    private static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

    }

    private void Start()
    {
        //StartCutscene1_3();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }


    private void Update()
    {
        CheckIfSceneChanged();
    }


    public void CheckIfSceneChanged()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 0) //Is in main menu
        {
            isInMainMenu = true;
            isInLevel = false;
        }
        else //Is in a level
        {
            isInMainMenu = false;
            isInLevel = true;
        }
    }

    public void OnChangeFromMenuToLevel()
    {
        if (isInMainMenu)
        {
            Menu.GetInstance().ChangeScene();
        }
    }

    public void ChangeToCutScene1_1()
    {
        SceneManager.LoadScene(2);
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


    #region CutScene
    public void StartCutscene1_1()
    {
        //Fade ind
        //Klods hans kommer ind, første bror kommer ind
        //Start bror 1 snak, dialog enter
        //Stop når han bliver afvist og inden han flytter sig igen

        Debug.Log("StartCutscene1_1");
        director.playableAsset = timelineAssets[0];
        director.Play();
    }

    public void StartCutscene1_2()
    {
        //Bror 1 snak done
        //Stop submit.
        //Flyt første bror hen til højre. 
        //Flyt anden bror hen til at gøre klar til at snakke.
        //Start anden bror snak, giv adgang til submit
        Debug.Log("StartCutscene1_2");
        director.playableAsset = timelineAssets[1];
        director.Play();
    }


    public void StartCutscene1_3()
    {
        //Bror 2 snak done
        //Stop submit.
        //Flyt bror 2 hen til højre
        //Start klods hans samtale. 
        //Giv adgang til submit
        Debug.Log("StartCutscene1_3");
        director.playableAsset = timelineAssets[2];
        director.Play();
    }

    public void StartCutscene1_4()
    {
        //Klods hans snak done
        //Dialog exit
        //Klods hans går til højre mod prinsessen, vender sig og går op mod hende.
        //Bg farve blir samme som menu, fade
        //Vis Vind menu
        Debug.Log("StartCutscene1_4");
        director.playableAsset = timelineAssets[3];
        director.Play();
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

