using UnityEngine;
using UnityEngine.InputSystem; //For at bruge det "nye" inputsystem

//Sørge for den kun virker når der er et PlayerInput
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    //Variabler til GetMetoder
    private Vector2 moveDirection = Vector2.zero; //WASD eller Piletasterne
    private Vector2 gridMoveDirection = Vector2.zero; //AD eller venstre og højre piletast

    private bool pausePressed = false; //Esc
    private bool interactPressed = false; //E
    private bool submitPressed = false; //R

    private static InputManager instance; //Sørge for at der kun kan være 1 på banen

    private void Awake()
    {
        //Sørger for at der kun kan være 1 på banen ved at sætte instance til this
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    //Static GetInstance for at kunne få adgang til dette script på alle scripts,
    //uden at først refere til den.
    public static InputManager GetInstance()
    {
        return instance;
    }

    //Bliver brugt til at kunne få moveDirection når spilleren trykker på enten W,A,S,D.
    public void MovePressed(InputAction.CallbackContext context)
    {
        //Performed er når man trykker på knappen. 
        //Et Check  for at ikke have noget dialog til at køre.
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            //Kortere version af hvordan man laver conditions mindre
            //God pratisk da man ikke inkupsulere ens kode i f.eks. 10 if statements.
            if (!GameManager.GetInstance().playerIsAllowedToMove) return;

            //Sætter Vector2 directions til hvad den læser
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled) //For at sikre at sætte direction til 0, 0.
        {
            moveDirection = Vector2.zero;
        }
    }

    public void GridMovePressed(InputAction.CallbackContext context)
    {
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (!GameManager.GetInstance().playerIsAllowedToMove) return;

            if (!PlayerController.GetInstance().gridMovement) return;

            gridMoveDirection = context.ReadValue<Vector2>();
            PlayerController.GetInstance().HandleGridMove(gridMoveDirection);
        }
        else if (context.canceled)
        {
            gridMoveDirection = Vector2.zero;
        }
    }


    //Til at tjekke om man trykker på pause. 
    public void PausePressed(InputAction.CallbackContext context)
    {
        //Tjekker ligesom ved direction, da man ikke skal kunne pause imens dialogue is player.
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            //Extra condition, man må ikke være i en cutscene.
            //Bruger også her GameMangers GetInstance til at få fat i scriptet.
            if (GameManager.GetInstance().isInCutScene) return;
            
            //Sætter til true for at kunne bruge den i Get Statemented
            pausePressed = true;
        }
        else if (context.canceled) //Sætter Pressed til false når man giver slip på knappen
        {
            pausePressed = false;
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
        }
        else if (context.canceled)
        {
            interactPressed = false;
        }
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        }
    }
    public Vector2 GetGridMoveDirection()
    {
        return gridMoveDirection;
    }

    //Get metoder
    //Kan blive kaldt man den retunere moveDirection til scriptet der kalder den
    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }


    //Sørger for at boolen er false,
    //efter at returnere en temp variable med samme værdi som pausePressed.
    public bool GetPausePressed()
    {
        bool result = pausePressed;
        pausePressed = false;
        return result;
    }

    //E
    public bool GetInteractPressed()
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    //Space
    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    //Har brug for en extra når det handler om dialogManageren.
    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }

}
