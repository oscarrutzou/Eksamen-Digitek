using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //Her defineres nogle variabler, som skal bruges senere i koden
    public Image actorImage; //variabel for at vise et billede af personen
    public TextMeshProUGUI actorName; //variabel for at vise navnet på personen
    public TextMeshProUGUI messageText; //variabel for at vise dialogen
    public RectTransform backgroundBox; //variabel for at vise baggrundsrammen omkring dialogen

    Message[] currentMessages; //et array til at opbevare de aktuelle beskeder i dialogen
    Actor[] currentActors; //et array til at opbevare de aktuelle NPC i dialogen
    int activeMessage = 0; //indeks for den aktive besked i dialogen
    public bool dialogIsActive = false; //boolsk værdi, der angiver om dialogen er aktiv

    //Variabler der kan sættes i Unity-editoren
    [SerializeField] private Animator animator; //variabel for at tilføje animationer til dialogen
    [SerializeField] private float animationTime = 0.5f; //variabel for at indstille længden af animationen

    [SerializeField] private float textFadeTime = 0.5f; //variabel for at indstille længden af tekstfadingen

    Color currentColor; //en variabel til at gemme den aktuelle farve for teksten

    //Metoden Start() kaldes, når programmet starter
    private void Start()
    {
        //Indstilling af baggrundsrammen ved at skalere den ned og skjule den
        backgroundBox.transform.localScale = Vector3.zero;
        backgroundBox.gameObject.SetActive(false);
    }

    //Metoden OpenDialogue() kaldes for at åbne dialogen og tage imod de relevante beskeder og NPC
    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        //Opdatering af aktuelle beskeder og NPC
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        dialogIsActive = true;

        DisplayMessage(); //Metoden DisplayMessage() kaldes for at vise den aktuelle besked
        backgroundBox.gameObject.SetActive(true); //Viser baggrundsrækken for dialogen

        //Bruger LeanTween til at skalere baggrundsrækken fra 0 til 1 i skala, så den bliver synlig
        backgroundBox.LeanScale(Vector3.one, animationTime).setEaseInOutExpo();
    }

    //Metoden DisplayMessage() tager den aktive besked i dialogen og viser den på skærmen
    private void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage]; //Vælger den aktuelle besked fra arrayet
        messageText.text = messageToDisplay.message; //Sætter teksten for den aktuelle besked

        Actor actorToDisplay = currentActors[messageToDisplay.actorId]; //Vælger den aktuelle NPC fra arrayet
        actorName.text = actorToDisplay.name; //Sætter navnet på den aktuelle NPC
        actorImage.sprite = actorToDisplay.sprite; //Sætter billedet på den aktuelle NPC

        AnimateTextColor(); //Starter en animation for teksten
    }

    //Metoden bliver kaldt af PlayerController når man trykker på interact knappen for at komme videre til næste besked.
    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage(); // Viser den næste besked, hvis der stadig er flere tilbage
        }
        else
        {
            dialogIsActive = false; // Deaktiverer dialogen, når der ikke er flere beskeder tilbage
            StartCoroutine(WaitForAnimationInOut()); // Venter på at animere dialogboksen ud, inden den deaktiveres
        }
    }

    private IEnumerator WaitForAnimationInOut()
    {
        backgroundBox.LeanScale(Vector3.zero, animationTime).setEaseInOutExpo(); // Skalerer dialogboksen fra 1 til 0
        yield return new WaitForSecondsRealtime(animationTime); // Venter på at animere færdig
        backgroundBox.gameObject.SetActive(false); // Deaktiverer dialogboksen
    }

    private void AnimateTextColor()
    {
        // Tager farven af current messageText så alphaen kan sættes til 0
        Color currentColor = messageText.color;
        currentColor.a = 0;

        messageText.color = currentColor;
        // Tween alfaen fra 0 til 1 over textFadeTime sekunder
        LeanTween.value(gameObject, UpdateAlpha, 0f, 1f, textFadeTime); // Animerer teksten for at fade ind
    }

    private void UpdateAlpha(float val)
    {
        // Get the current color of the text
        currentColor = messageText.color;
        // Set the alpha to the tweened value
        currentColor.a = val;
        messageText.color = currentColor;
    }

}

