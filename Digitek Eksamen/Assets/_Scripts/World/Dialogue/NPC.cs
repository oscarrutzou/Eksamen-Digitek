using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{

    [SerializeField] private DialogueTrigger dialogueTrigger;


    public override void NPCInteract()
    {
        dialogueTrigger.StartDialogue();
        //trigger.StartDialogue();
    }
}
