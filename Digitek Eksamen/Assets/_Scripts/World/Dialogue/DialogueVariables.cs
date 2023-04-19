using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    private Story globalVariablesStory;

    private string saveVariablesKey = "INK_VARIABLES";

    public DialogueVariables(TextAsset loadGlobalsJSON)
    {
        //Create the story
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        //Hvis der er data, load det
        if (PlayerPrefs.HasKey(saveVariablesKey))
        {
            string jsonState = PlayerPrefs.GetString(saveVariablesKey);
            //Debug.Log("Load Save");
            globalVariablesStory.state.LoadJson(jsonState);
        }

        //Initialize dictionary
        InitializeDictionary();

    }

    private void InitializeDictionary()
    {
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            //Debug.Log("Initialized global dialogue variable:" + name + " = " + value);
        }
    }

    public void SaveVariables()
    {
        if (globalVariablesStory != null)
        {
            // Load the current state of all of our variables to the globals story
            VariablesToStory(globalVariablesStory);
            // Replace this with an actual save/load method
            
            PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
        }
    }

    public void DeleteSavedVariables()
    {
        DialogueManager.GetInstance().SetVariableState("questItemsCollected", new Ink.Runtime.IntValue(0));

        DialogueManager.GetInstance().SetVariableState("hc_firstTalkCalled", new Ink.Runtime.BoolValue(false));

        DialogueManager.GetInstance().SetVariableState("hc_secondTalkCalled", new Ink.Runtime.BoolValue(false));

        DialogueManager.GetInstance().SetVariableState("brothers_firstTalkCalled", new Ink.Runtime.BoolValue(false));

        DialogueManager.GetInstance().SetVariableState("brothers_secondTalkCalled", new Ink.Runtime.BoolValue(false));
        
        DialogueManager.GetInstance().SetVariableState("hasFinnishedGridLvl", new Ink.Runtime.BoolValue(false));


        SaveVariables();
        InitializeDictionary();
    }

    public void StartListening(Story story)
    {

        VariablesToStory(story); //Vigtigt den bliver kaldt her eller kommer der fejl.
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        // Maintainer kun variabler som er fra globals ink fil
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

}
