using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatInteractable : Interactable
{
    private PlayerController playerController;

    public override void ObjectInteract()
    {
        playerController = PlayerController;

        Debug.Log("Hop p� ged");

        //Check om kan hoppe p� ged - quest done

        //�ndre animation
        //
        //brug controllers goat speed


        // Implementation for GoatInteractable object
        //PlayerController.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}


