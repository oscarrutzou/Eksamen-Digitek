using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatInteractable : Interactable
{
    //private PlayerController playerController;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public override void ObjectInteract()
    {
        if (((Ink.Runtime.IntValue)DialogueManager.GetInstance().GetVariableState("questItemsCollected")).value >= 2)
        {
            gameManager.StartMountMovement();
            gameObject.SetActive(false);
        }
    }
}


