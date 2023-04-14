using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController PlayerController;
    public PlayerInteract PlayerInteract;
    public Menu Menu;
    public AudioManager AudioManager;

    public bool isInMainMenu { get; set; }
    public bool isInLevel { get; set; }

    private static GameManager instance;

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

    public void CutSceneMeetBrothersLvl1()
    {
        Debug.Log("Cutscene her");
    }

    public void PlayerDead()
    {
        Menu.OnDeathMenu(); //Klarer menu delen


    }

}

