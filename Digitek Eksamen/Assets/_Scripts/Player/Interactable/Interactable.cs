using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public static PlayerController PlayerController;
    public static GameManager GameManager;

    private void Start()
    {
        PlayerController = FindObjectOfType<PlayerController>();
        GameManager = GetComponent<GameManager>();
    }

    public virtual void ObjectInteract()
    {
        // Base implementation for all interactable objects
    }

    public virtual void QuestItemInteract()
    {
        // Base implementation for all interactable objects
    }

    public virtual void NPCInteract()
    {
        // Base implementation for all interactable NPCs
    }
}

