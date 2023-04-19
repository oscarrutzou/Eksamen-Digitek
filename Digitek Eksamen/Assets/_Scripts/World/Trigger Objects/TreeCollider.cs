using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCollider : MonoBehaviour
{

    private void Update()
    {
        if (((Ink.Runtime.IntValue)DialogueManager.GetInstance().GetVariableState("questItemsCollected")).value == 2)
        {
            this.gameObject.SetActive(false);
        }
    }


}
