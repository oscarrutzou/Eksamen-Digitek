using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //Her defineres nogle variabler, som skal bruges senere i koden
    public Image actorImage; //variabel for at vise et billede af personen
    public TextMeshProUGUI actorName; //variabel for at vise navnet p� personen
    public TextMeshProUGUI messageText; //variabel for at vise dialogen
    public RectTransform backgroundBox; //variabel for at vise baggrundsrammen omkring dialogen

    Message[] currentMessages; //et array til at opbevare de aktuelle beskeder i dialogen
    Actor[] currentActors; //et array til at opbevare de aktuelle NPC i dialogen
    int activeMessage = 0; //indeks for den aktive besked i dialogen
    public bool dialogIsActive = false; //boolsk v�rdi, der angiver om dialogen er aktiv

    //Variabler der kan s�ttes i Unity-editoren
    [SerializeField] private Animator animator; //variabel for at tilf�je animationer til dialogen
    [SerializeField] private float animationTime = 0.5f; //variabel for at indstille l�ngden af animationen

    [SerializeField] private float textFadeTime = 0.5f; //variabel for at indstille l�ngden af tekstfadingen

    Color currentColor; //en variabel til at gemme den aktuelle farve for teksten

    //Metoden Start() kaldes, n�r programmet starter
    private void Start()
    {
        //Indstilling af baggrundsrammen ved at skalere den ned og skjule den
        backgroundBox.transform.localScale = Vector3.zero;
        backgroundBox.gameObject.SetActive(false);
    }

    //Metoden OpenDialogue() kaldes for at �bne dialogen og tage imod de relevante beskeder og NPC
    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        //Opdatering af aktuelle beskeder og NPC
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        dialogIsActive = true;

        DisplayMessage(); //Metoden DisplayMessage() kaldes for at vise den aktuelle besked
        backgroundBox.gameObject.SetActive(true); //Viser baggrundsr�kken for dialogen

        //Bruger LeanTween til at skalere baggrundsr�kken fra 0 til 1 i skala, s� den bliver synlig
        backgroundBox.LeanScale(Vector3.one, animationTime).setEaseInOutExpo();
    }

    //Metoden DisplayMessage() tager den aktive besked i dialogen og viser den p� sk�rmen
    private void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage]; //V�lger den aktuelle besked fra arrayet
        messageText.text = messageToDisplay.message; //S�tter teksten for den aktuelle besked

        Actor actorToDisplay = currentActors[messageToDisplay.actorId]; //V�lger den aktuelle NPC fra arrayet
        actorName.text = actorToDisplay.name; //S�tter navnet p� den aktuelle NPC
        actorImage.sprite = actorToDisplay.sprite; //S�tter billedet p� den aktuelle NPC

        AnimateTextColor(); //Starter en animation for teksten
    }

    //Metoden bliver kaldt af PlayerController n�r man trykker p� interact knappen for at komme videre til n�ste besked.
    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage(); // Viser den n�ste besked, hvis der stadig er flere tilbage
        }
        else
        {
            dialogIsActive = false; // Deaktiverer dialogen, n�r der ikke er flere beskeder tilbage
            StartCoroutine(WaitForAnimationInOut()); // Venter p� at animere dialogboksen ud, inden den deaktiveres
        }
    }

    private IEnumerator WaitForAnimationInOut()
    {
        backgroundBox.LeanScale(Vector3.zero, animationTime).setEaseInOutExpo(); // Skalerer dialogboksen fra 1 til 0
        yield return new WaitForSecondsRealtime(animationTime); // Venter p� at animere f�rdig
        backgroundBox.gameObject.SetActive(false); // Deaktiverer dialogboksen
    }

    private void AnimateTextColor()
    {
        // Tager farven af current messageText s� alphaen kan s�ttes til 0
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

