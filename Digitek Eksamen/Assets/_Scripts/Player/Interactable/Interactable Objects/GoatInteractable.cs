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

        Debug.Log("Hop på ged");

        //Check om kan hoppe på ged - quest done

        //Ændre animation
        //
        //brug controllers goat speed


        // Implementation for GoatInteractable object
        //PlayerController.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}


