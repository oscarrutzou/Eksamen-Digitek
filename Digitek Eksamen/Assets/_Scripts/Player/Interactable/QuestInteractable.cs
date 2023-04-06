using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInteractable : Interactable
{
    public override void QuestItemInteract()
    {
        Ink.Runtime.IntValue newValue = new Ink.Runtime.IntValue(2);
        DialogueManager.GetInstance().SetVariableState("questItemsCollected", newValue);

        gameObject.SetActive(false);
    }

}
