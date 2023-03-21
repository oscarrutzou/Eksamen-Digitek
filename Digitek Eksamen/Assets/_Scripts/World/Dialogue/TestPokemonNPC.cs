using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
public class TestPokemonNPC : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color charmanderColor = Color.red;
    [SerializeField] private Color bulbasaurColor = Color.green;
    [SerializeField] private Color squirtleColor = Color.blue;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        //Hvis valuen ikke er en string så cast den indtil den rigtige value f.eks. float.
        string pokemonName = ((Ink.Runtime.StringValue)DialogueManager.GetInstance().GetVariableState("pokemon_name")).value;
        Debug.Log(pokemonName);

        switch (pokemonName)
        {
            case "":
                spriteRenderer.color = defaultColor;
                break;
            case "Charmander":
                spriteRenderer.color = charmanderColor;
                break;
            case "Bulbasaur":
                spriteRenderer.color = bulbasaurColor;
                break;
            case "Squirtle":
                spriteRenderer.color = squirtleColor;
                break;
            default:
                Debug.LogWarning("Pokemon name not handled by switch statement: " + pokemonName);
                break;
        }
    }
}
