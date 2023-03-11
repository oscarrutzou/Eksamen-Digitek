using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Image actorImage;
    public TextMeshProUGUI actorName;
    public TextMeshProUGUI messageText;
    public RectTransform backgroundBox;

    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public bool isActive = false;

    private void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
    }

    //Find ud af at have flere actors og lav s� quest system kan holde �ve med hvor den er. Skal kunne lave dialog om


    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;

        Debug.Log("Started, loaded messages : " + messages.Length);
        DisplayMessage();
        backgroundBox.LeanScale(Vector3.one, 0.5f).setEaseInOutExpo();
    }

    private void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage]; //Tager den f�rste
        messageText.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.name;
        actorImage.sprite = actorToDisplay.sprite;

        //AnimateTextColor();
    }

    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Convo enden");
            isActive = false;
            backgroundBox.LeanScale(Vector3.zero, 0.5f).setEaseInOutExpo();
        }
    }

    //private void AnimateTextColor()
    //{
    //    LeanTween.textAlpha(messageText.rectTransform, 0, 0);
    //    LeanTween.textAlpha(messageText.rectTransform, 1, 1.5f);
    //    Debug.Log("Anim text");
    //}

}

