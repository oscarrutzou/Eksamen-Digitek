using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// En klasse, der udløser dialoger, som er knyttet til et spilobjekt
public class DialogueTrigger : MonoBehaviour
{
    // En offentlig variabel, der indeholder alle beskeder, der skal vises under dialogen
    public Message[] messages;
    // En offentlig variabel, der indeholder alle NPCerne i dialogen
    public Actor[] actors;
    // En metode, der udløser dialogen ved at kalde OpenDialogue metoden i DialogueManager klassen
    public void StartDialogue()
    {
        FindObjectOfType<DialogueManager>().OpenDialogue(messages, actors); //Det er muligt at gøre det med singletons
    }
}

// En klasse, der indeholder en besked
[System.Serializable]
public class Message
{
    // ID'en af den NPC, der sender beskeden
    public int actorId;
    // Beskedteksten
    public string message;
}

// En klasse, der repræsenterer en NPC i dialogen
[System.Serializable]
public class Actor
{
    // NPCens navn
    public string name;
    // NPCens sprite, der bruges til at repræsentere dem i dialogen
    public Sprite sprite;
}