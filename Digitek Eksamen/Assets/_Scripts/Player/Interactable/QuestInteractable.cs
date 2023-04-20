using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInteractable : Interactable
{
    [SerializeField] private string soundClipName;

    public override void QuestItemInteract()
    {
        int currentValue = ((Ink.Runtime.IntValue)DialogueManager.GetInstance().GetVariableState("questItemsCollected")).value;

        Ink.Runtime.IntValue newValue = new Ink.Runtime.IntValue(currentValue + 1);
        DialogueManager.GetInstance().SetVariableState("questItemsCollected", newValue);

        if (soundClipName != null)
        {
            AudioManager.GetInstance().Play(soundClipName);
        }
        else
        {
            Debug.LogWarning("No soundclip in gameobject: " + gameObject.name);
        }

        gameObject.SetActive(false);
    }

}
