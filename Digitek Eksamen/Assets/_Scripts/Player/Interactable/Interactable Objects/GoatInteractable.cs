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
        if (((Ink.Runtime.IntValue)DialogueManager.GetInstance().GetVariableState("questItemsCollected")).value >= 2)
        {
            playerController = PlayerController;
            gameManager = GameManager;

            gameManager.StartMountMovement();

            gameObject.SetActive(false);
        }
    }
}


