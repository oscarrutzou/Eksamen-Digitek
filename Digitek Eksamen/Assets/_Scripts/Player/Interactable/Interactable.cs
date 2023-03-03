using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public static PlayerController PlayerController;
    public static GameManager GameManager;
    [SerializeField] private bool isQuestItem = false;

    private void Start()
    {
        PlayerController = FindObjectOfType<PlayerController>();
        GameManager = GetComponent<GameManager>();
    }

    public virtual void ObjectInteract()
    {
        
        if (isQuestItem)
        {
            Debug.Log("Do something");
        }
        
        // Base implementation for all interactable objects
        //
    }

    public virtual void NPCInteract()
    {
        // Base implementation for all interactable NPCs
    }
}

