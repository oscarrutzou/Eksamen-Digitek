using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObjectHover : MonoBehaviour
{
    [Header("NPC Hover Icons")]
    //[SerializeField] private GameObject[] gameObjectNpcIcon;
    [SerializeField] private List<string> nameNpc;
    [SerializeField] private List<GameObject> npcIconGameObject;

    [SerializeField] private List<Animator> animatorNpcIcon;

    [Header("Quest Items Indicator")]
    [SerializeField] private List<string> nameQuestItem;
    [SerializeField] private List<GameObject> questGameObject;

    private void Start()
    {

        for (int i = 0; i < npcIconGameObject.Count; i++)
        {
            animatorNpcIcon[i] = npcIconGameObject[i].GetComponent<Animator>();
        }

        //OnOffQuestPopUp("Shoe", false);
        //OnOffNpcPopUp("Andersen", false);
    }

    public void OnOffQuestPopUp(string questObjectName, bool onOff)
    {
        ///Metoden IndexOf returnerer indekset for den første af det angivne element i listen. 
        ///Hvis elementet ikke findes i listen, returnerer IndexOf -1.
        int slotInList = nameQuestItem.IndexOf(questObjectName);

        if (slotInList != -1)
        {
            // questName was found in the list at index slotInList
            questGameObject[slotInList].SetActive(onOff);
        }
        else
        {
            // questName was not found in the list
            Debug.LogWarning("Navnet er skrevet forkert på quest");
        }
    }

    public void OnOffNpcPopUp(string npcName, bool onOff)
    {
        ///Metoden IndexOf returnerer indekset for den første af det angivne element i listen. 
        ///Hvis elementet ikke findes i listen, returnerer IndexOf -1.
        int slotInList = nameNpc.IndexOf(npcName);

        if (slotInList != -1)
        {
            // npcName was found in the list at index slotInList
            npcIconGameObject[slotInList].SetActive(onOff);
        }
        else
        {
            // npcName was not found in the list
            Debug.LogWarning("Navnet er skrevet forkert på den som kalder animationen");
        }
    }
}
