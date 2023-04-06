using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController PlayerController;
    public PlayerInteract PlayerInteract;
    public Menu Menu;
    public AudioManager AudioManager;


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


    //Hvornår man må slå special movement and grid movement fra.


    //Gør dette igennem gamemanager, fordi det hjælper på at holde styr på alt. Siden der også skal være andre elementer.
    //Skal holde styr på at man først er på fods - så ged - temple run - så ged - fods.
    //Lav statier i spillet - kig på level og hvad statie der er i den. Bare plus dem, så der kan holdes styr sådan.

    //Lavet så der kan ske andre ting når man starter og stopper. Måske transitions eller lign.
    //Bliver sat ved trigger colliders, når de skal starte og stoppe.
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

    public void CutSceneMeetBrothersLvl1()
    {
        Debug.Log("Cutscene her");
    }

    public void PlayerDead()
    {
        Menu.OnDeathMenu(); //Klarer menu delen


    }

}
