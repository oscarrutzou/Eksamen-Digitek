using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatInteractable : Interactable
{
    private PlayerController playerController;
    private GameManager gameManager;

    public override void ObjectInteract()
    {
        playerController = PlayerController;
        gameManager = GameManager;

        Debug.Log("Hop på ged");

        //Check om kan hoppe på ged - quest done

        //Ændre animation
        //
        //brug controllers goat speed
        gameManager.StartSpecialMovement();

        // Implementation for GoatInteractable object
        //PlayerController.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}


