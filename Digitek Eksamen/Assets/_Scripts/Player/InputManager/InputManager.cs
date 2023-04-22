using UnityEngine;
using UnityEngine.InputSystem; //For at bruge det "nye" inputsystem

//S�rge for den kun virker n�r der er et PlayerInput
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    //Variabler til GetMetoder
    private Vector2 moveDirection = Vector2.zero; //WASD eller Piletasterne
    private Vector2 gridMoveDirection = Vector2.zero; //AD eller venstre og h�jre piletast

    private bool pausePressed = false; //Esc
    private bool interactPressed = false; //E
    private bool submitPressed = false; //R

    private static InputManager instance; //S�rge for at der kun kan v�re 1 p� banen

    private void Awake()
    {
        //S�rger for at der kun kan v�re 1 p� banen ved at s�tte instance til this
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    //Static GetInstance for at kunne f� adgang til dette script p� alle scripts,
    //uden at f�rst refere til den.
    public static InputManager GetInstance()
    {
        return instance;
    }

    //Bliver brugt til at kunne f� moveDirection n�r spilleren trykker p� enten W,A,S,D.
    public void MovePressed(InputAction.CallbackContext context)
    {
        //Performed er n�r man trykker p� knappen. 
        //Et Check  for at ikke have noget dialog til at k�re.
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            //Kortere version af hvordan man laver conditions mindre
            //God pratisk da man ikke inkupsulere ens kode i f.eks. 10 if statements.
            if (!GameManager.GetInstance().playerIsAllowedToMove) return;

            //S�tter Vector2 directions til hvad den l�ser
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled) //For at sikre at s�tte direction til 0, 0.
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


    //Til at tjekke om man trykker p� pause. 
    public void PausePressed(InputAction.CallbackContext context)
    {
        //Tjekker ligesom ved direction, da man ikke skal kunne pause imens dialogue is player.
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            //Extra condition, man m� ikke v�re i en cutscene.
            //Bruger ogs� her GameMangers GetInstance til at f� fat i scriptet.
            if (GameManager.GetInstance().isInCutScene) return;
            
            //S�tter til true for at kunne bruge den i Get Statemented
            pausePressed = true;
        }
        else if (context.canceled) //S�tter Pressed til false n�r man giver slip p� knappen
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


    //S�rger for at boolen er false,
    //efter at returnere en temp variable med samme v�rdi som pausePressed.
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

    //Har brug for en extra n�r det handler om dialogManageren.
    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }

}
