using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 gridMoveDirection = Vector2.zero;

    private bool pausePressed = false;
    private bool interactPressed = false;
    private bool submitPressed = false;
    private bool jumpPressed = false;

    private static InputManager instance;

    public PlayerController playerController;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    public static InputManager GetInstance()
    {
        return instance;
    }

    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (!GameManager.GetInstance().playerIsAllowedToMove) return;

            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveDirection = context.ReadValue<Vector2>();
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

    public void JumpPressed(InputAction.CallbackContext context)
    {
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (playerController == null) return;
            jumpPressed = true;
        }
        else if (context.canceled)
        {
            jumpPressed = false;
        }
    }


    public void PausePressed(InputAction.CallbackContext context)
    {
        if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (playerController == null) return;
            pausePressed = true;
        }
        else if (context.canceled)
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

    //Get metoder
    //Arrows eller WASD
    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    public Vector2 GetGridMoveDirection()
    {
        return gridMoveDirection;
    }

    //Esc
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

    //Space
    public bool GetJumpPressed()
    {
        bool result = jumpPressed;
        jumpPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }

}
