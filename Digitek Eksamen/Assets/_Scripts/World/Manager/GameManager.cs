using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController PlayerController;
    public PlayerInteract PlayerInteract;
    public Menu Menu;
    public AudioManager AudioManager;
    
    


    private void Start()
    {
        
    }

    //Hvorn�r man m� sl� special movement and grid movement fra.


    //G�r dette igennem gamemanager, fordi det hj�lper p� at holde styr p� alt. Siden der ogs� skal v�re andre elementer.
    //Skal holde styr p� at man f�rst er p� fods - s� ged - temple run - s� ged - fods.
    //Lav statier i spillet - kig p� level og hvad statie der er i den. Bare plus dem, s� der kan holdes styr s�dan.

    //Lavet s� der kan ske andre ting n�r man starter og stopper. M�ske transitions eller lign.
    //Bliver sat ved trigger colliders, n�r de skal starte og stoppe.
    public void StartMountMovement()
    {
        //Starter special mov, alts� ged mov
        PlayerController.MountMovement(true);
    }
    public void StopMountMovement()
    {
        //Stopper special mov, alts� ged mov
        PlayerController.MountMovement(false);
    }


    public void StartGridMovement()
    {
        //Start grid mov
        PlayerController.GridMovement(true);
    }

    public void StopGridMovement()
    {
        //Stopper grid mov
        PlayerController.GridMovement(false);
    }

}
