using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrotherCollider : MonoBehaviour
{
    [SerializeField] private GameObject BlockPlayerCollider;

    private void Start()
    {
        BlockPlayerCollider.SetActive(true);
    }

    private void Update()
    {
        if (((Ink.Runtime.IntValue)DialogueManager.GetInstance().GetVariableState("questItemsCollected")).value == 3
            && ((Ink.Runtime.BoolValue)DialogueManager.GetInstance().GetVariableState("brothers_firstTalkCalled")).value == true)
        {
            BlockPlayerCollider.SetActive(false);
        }
    }

}
