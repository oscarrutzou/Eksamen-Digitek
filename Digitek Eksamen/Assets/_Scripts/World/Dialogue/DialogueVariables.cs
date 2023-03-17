using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    private Story globalVariablesStory;

    private const string saveVariablesKey = "INK_VARIABLES";

    public DialogueVariables(TextAsset loadGlobalsJSON)
    {
        //Create the story
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        //Hvis der er data, load det
        if (PlayerPrefs.HasKey(saveVariablesKey))
        {
            Debug.Log("HASKEY");
            string jsonState = PlayerPrefs.GetString(saveVariablesKey);
            globalVariablesStory.state.LoadJson(jsonState);
        }

        //Initialize dictionary
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
            VariablesToStory(globalVariablesStory);
            PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToString()); //Kan ændre til et andet save/load metode 
            Debug.Log("TEst save variables");
        }
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
