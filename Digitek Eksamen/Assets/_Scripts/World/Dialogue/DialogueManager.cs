using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.Audio;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.02f;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    [SerializeField] private bool makePredictable;
    [SerializeField] private AudioMixerGroup dialogueMixerGroup;

    public AudioSource audioSource;


    private DialogueAudioInfoSO currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanel;

    private string allLines;
    private Story currentStory;

    private string nextLine;

    public bool dialogueIsPlaying { get; private set; } //Kun andre scripts kan læse men ikke ændre på den.
    private bool canContinueToNextLine = false;

    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;


    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";
    private const string AUDIO_TAG = "audio";

    public DialogueVariables dialogueVariables;

    public Menu menu;


    private bool isIntro = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);

        audioSource = this.gameObject.AddComponent<AudioSource>();
        
        currentAudioInfo = defaultAudioInfo;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
        InitializeAudioInfoDictionary();

        audioSource.outputAudioMixerGroup = dialogueMixerGroup;

        dialogueVariables.DeleteSavedVariables();
    }

    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        foreach (DialogueAudioInfoSO audioInfo in audioInfos)
        {
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }
    }

    private void SetCurrentAudioInfo(string id)
    {
        DialogueAudioInfoSO audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            this.currentAudioInfo = audioInfo;
        }
        else
        {
            Debug.LogWarning("Failed to find audio info for id: " + id);
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (canContinueToNextLine && currentStory.currentChoices.Count == 0)
        {
            //if (isIntro)
            //{
            //    ContinueIntroStory();
            //}
            if (isIntro || InputManager.GetInstance().GetSubmitPressed())
            {
                ContinueStory();
            }
        }
    }


    //Kaldt når man skal starte en dialog
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        //Sætter Story (fra ink) til den man får givet.
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true; //Sørger for at alle ved at dialog er i gang
        dialoguePanel.SetActive(true); //Activere dialog panelet.

        dialogueVariables.StartListening(currentStory); //Til variabler senere 

        //Restter portræt, layout and navn
        displayNameText.text = "Name";
        layoutAnimator.Play("right");
        dialogueText.text = "";

        ContinueStory(); //Starter story
    }

    //EP 2, 21.10 hvis der skal være jump med i spillet. Så det ikke overlapper
    public void ExitDialogueMode()
    {
        if (!isIntro)
        {
            dialogueVariables.StopListening(currentStory);

            dialogueIsPlaying = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";

            //Sætter default audio på for fejl
            SetCurrentAudioInfo(defaultAudioInfo.id);
        } else if (isIntro)
        {
            isIntro = false;
            GameManager.GetInstance().OnChangeFromMenuToLevel();
        }
    }

    private void ContinueStory()
    {
        //canContinue er fra Ink og tjekker om der er mere i Storyen
        if (currentStory.canContinue)
        {
            // Stopper Coroutinenen hvis den har været i gang.
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            //Et variable til at kende det næste der sker i Storyen
            nextLine = currentStory.Continue();

            // Tjekker tags i gennem fra linjen.
            HandleTags(currentStory.currentTags);

            //Tjekker om det er dialog som spillet som introen til levelet eller ej
            if (isIntro)
            {
                //Stopper menuPanel og starter coroutine DisplayIntro
                //Den er ligesom DisplayLine coroutine,
                //men viser alt teksten under hinanden uden at slette det tidligere tekst.
                menuPanel.SetActive(false);
                displayLineCoroutine = StartCoroutine(DisplayIntro(nextLine));
                return;
            }
            else
            {
                //Hvis linjen ved en coroutine som hedder DisplayLine
                //Den viser kun næste linje.
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        //Hvis der ikke er mere test og man trykker på submit så går man ud af dialog moden.
        else if (!isIntro || InputManager.GetInstance().GetSubmitPressed())
        {
            ExitDialogueMode();
        }
    }

    private void GetAllLines()
    {
        allLines = "";
        allLines += dialogueText.text;

        for (int i = 0; i < 10; i++)
        {
            if (currentStory.canContinue)
            {
                allLines += currentStory.Continue();

            }
            else
            {
                break;
            }
        }

        //Debug.Log(allLines);

        //callOnceDisplayIntro = true;

        dialogueText.text = allLines;
        dialogueText.maxVisibleCharacters = allLines.Length;
    }

    private IEnumerator DisplayIntro(string line)
    {
        // Keep the previous line and append the new line
        dialogueText.text += line; // "\n"+
        dialogueText.maxVisibleCharacters = 0;

        if (dialogueText.maxVisibleCharacters == 0)
        {
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
            //Debug.Log("dialogueText.maxVisibleCharacters>EOR" + dialogueText.maxVisibleCharacters);
        }
        else
        {
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount - line.Length;
        }

        //Debug.Log("dialogueText.maxVisibleCharacters" + dialogueText.maxVisibleCharacters);
        // Everything that should be hidden while typing
        continueIcon.SetActive(false);
        HideChoices();
        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;
        foreach (char letter in line.ToCharArray())
        {
            // If submit pressed, write the whole line at once
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                GetAllLines();
                break; // Exits foreach loop
            }

            // Checks for rich text tag and adds it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else // If it's not a rich text tag, adds letters after waiting a bit
            {
                PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }


        if (!currentStory.canContinue)
        {
            continueIcon.SetActive(true);
            //Debug.Log("!canContinue");
        }
        else
        {
            //Debug.Log("canContinue");
            yield return new WaitForSeconds(1.5f);
        }

        //callOnceDisplayIntro = true;
        canContinueToNextLine = true;
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        // Everything that should be hidden while typing
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        foreach (char letter in line.ToCharArray())
        {
            // If submit pressed, write the whole line at once
            if (InputManager.GetInstance().GetSubmitPressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break; // Exits foreach loop
            }

            // Checks for rich text tag and adds it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else // If it's not a rich text tag, adds letters after waiting a bit
            {
                PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // Shows everything that was hidden again
        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
    }


    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        // set variables for the below based on our config
        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;


        //Moduller division baseret på config
        if (currentDisplayedCharacterCount % frequencyLevel == 0 )
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            AudioClip soundClip = null;
            //Lav predictable audio fra hashing
            if (makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                // sound clip
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];
                // pitch
                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;

                // cannot divide by 0, so if there is no range then skip the selection
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
                else
                {
                    audioSource.pitch = minPitch;
                }
            }
            else
            {
                //Random Sound Clip
                int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];

                //Random pitch
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }

            audioSource.PlayOneShot(soundClip);
        }
    }


    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    //Tjekker Tags
    private void HandleTags(List<string> currentTags)
    {
        //Går igennem hvert tag
        foreach (string tag in currentTags)
        {
            //Parser tags og tjekker om man godt kan parse dem ordenlidt
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag kunne ikke ordenlig blive parsed: " + tag);
            }
            //Finder Key og valuen og sætter dem til forskellige dele af arrayet.
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            //Går gennem alle forskellige tagKeys og tjekker om hvad de er
            switch (tagKey)
            {
                //Sætter name teksten til valuen
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                //Sætter portættet til den rigtige value.
                //Den bruger animtoren til at vælge de forskellige portrætter.
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                //Tjekker de forskellige layouts f.eks. om det skal være en intro layout
                case LAYOUT_TAG:
                    CheckForIntro(tagValue);
                    break;
                //Sætter lyden som de siger til den rette value
                case AUDIO_TAG:
                    SetCurrentAudioInfo(tagValue);
                    break;
                //Hvis der var nogle tags som ikke havde en plads i switch så bliver dette kaldt
                default:
                    Debug.LogWarning("Tag kom ind men kunne ikke håndteres: " + tag);
                    break;
            }
        }
    }

    private void CheckForIntro(string tag)
    {
        if (tag == "book")
        {
            //Debug.Log("Det er en intro");
            isIntro = true;
        }

        layoutAnimator.Play(tag);
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices that the UI can support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        // Activere alle choices up til hvor mange der burde være.
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    // Til UI navigation mellem choices
    private IEnumerator SelectFirstChoice()
    {
        // Nogle problemer med Event System som gør at man først skal fjerne den, vente
        // mindst 1 frame før man kan sætte current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            // NOTE: The below two lines were added to fix a bug after the Youtube video was made
            InputManager.GetInstance().RegisterSubmitPressed();
            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink variable was found to be null: " + variableName);
        }
        return variableValue;
    }

    public void SetVariableState(string variableName, Ink.Runtime.Object variableValue)
    {
        if (dialogueVariables.variables.ContainsKey(variableName))
        {
            if (dialogueVariables.variables[variableName] != variableValue)
            {
                dialogueVariables.variables[variableName] = variableValue;
                //Debug.Log(dialogueVariables.variables[variableName]);
            }
            else
            {
                Debug.Log("Ink variable value is already the same: " + variableName);
            }
        }
        else
        {
            Debug.LogWarning("Ink variable not found: " + variableName);
        }
    }

    //Saver variabler når den lukker
    public void OnApplicationQuit()
    {
        if (dialogueVariables != null)
        {
            dialogueVariables.SaveVariables();
        }
    }


}

