using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrotherCollider : MonoBehaviour
{


    [SerializeField] private GameObject BrothersGameObjects;

    [SerializeField] private GameObject BlockPlayerCollider;

    private void Start()
    {
        BrothersGameObjects.SetActive(false);
        BlockPlayerCollider.SetActive(true);
    }

    private void Update()
    {
        if (((Ink.Runtime.IntValue)DialogueManager.GetInstance().GetVariableState("questItemsCollected")).value == 3)
        {
            BrothersGameObjects.SetActive(true);

            if (((Ink.Runtime.BoolValue)DialogueManager.GetInstance().GetVariableState("brothers_firstTalkCalled")).value == true)
            {
                BlockPlayerCollider.SetActive(false);
            }
        }
    }

}
