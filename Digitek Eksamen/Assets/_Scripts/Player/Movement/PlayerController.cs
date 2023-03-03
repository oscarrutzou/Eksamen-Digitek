using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Menu menu;
    public GameManager gameManager;
    public PlayerInputActions playerInputActions;
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    private Rigidbody2D rb;

    public float moveSpeed = 1f;
    public float tempMoveSpeed;
    [SerializeField] float collisionOffset = 0.05f;
    [SerializeField] ContactFilter2D movementFilter;

    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public float lastXInput;
    [HideInInspector] public float lastYInput;

    private InputAction interact;
    private InputAction pause;
    private InputAction jump;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();


    //Goat
    //Interact med ged, 
    private PlayerInteract playerInteract;
    public bool specialMovement;
    public bool gridMovement;
    private bool unlockedJump;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInteract = GetComponent<PlayerInteract>();

        specialMovement = false;
        gridMovement = false;
        unlockedJump = false;
        tempMoveSpeed = moveSpeed;
    }

    #region Enable + Disable
    private void OnEnable()
    {
        pause = playerInputActions.Player.Pause;
        pause.Enable();
        pause.performed += Pause;

        interact = playerInputActions.Player.Interact;
        interact.Enable();
        interact.performed += Interact;

        jump = playerInputActions.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        pause.Disable();
        interact.Disable();
        jump.Disable();
    }
    #endregion

    private void FixedUpdate()
    {
        if (specialMovement && gridMovement)
        {
            Debug.LogError("Cant have both special and grid movement on, on the same time");
        }

        #region Movement Input
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
                //Debug.Log("Movementinput.x");

            }

            if (!success)
            {
                success = TryMove(new Vector2(0, movementInput.y));
                //Debug.Log("Movementinput.y");
            }

            if (!specialMovement && !gridMovement)
            {
                //normal movement.
                animator.SetBool("isMoving", success);
            }
            else if (specialMovement || gridMovement)
            {
                //Lav om til goat animation
                animator.SetBool("isMoving", success);
            }
            
        }
        else
        {
            if (!specialMovement && !gridMovement)
            {
                //normal movement
                animator.SetBool("isMoving", false);
            }
            else if (specialMovement || gridMovement)
            {
                //Lav om til goat animation
                animator.SetBool("isMoving", false);
            }
        }

        if (movementInput.x != 0)
        {
            lastXInput = movementInput.x;
        }
        else if (movementInput.y != 0)
        {
            lastYInput = movementInput.y;
        }

        if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        #endregion


    }


    #region Movement
    private bool TryMove(Vector2 direction)
    {

        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                    movementInput,
                    movementFilter,
                    castCollisions,
                    moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + movementInput * tempMoveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }
    private void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }
    #endregion

    //Ged movement
    public void SpecialMovement(bool active)
    {
        if (active)
        {
            Debug.Log("Special mov on");
            specialMovement = true;
            tempMoveSpeed = moveSpeed * 1.5f; //Sæt speed for movement med ged
            //Ændre animation
        }
        else if (!active)
        {
            Debug.Log("Special mov off");
            specialMovement = false;
            tempMoveSpeed = moveSpeed;
            //Ændre animation.
        }
    }


    public void GridMovement(bool active)
    {
        if (active)
        {
            Debug.Log("GridMovement mov on");
            
        }
        else if (!active)
        {
            Debug.Log("GridMovement mov off");
            
        }
    }


    private void Jump(InputAction.CallbackContext ctx)
    {
        Debug.Log(" jumped");

        
        unlockedJump = true; //gør intet, bare for at fjerne debug tingen
        unlockedJump = false; //gør intet, bare for at fjerne debug tingen
        //Check if collideren er jumpable, hvis ja, så jump animation når man går ind i den. Find ud af hvad der sker når den går ned.
    }
    
    private void Pause(InputAction.CallbackContext ctx)
    {

        gameManager.StopSpecialMovement();
        //if (menu.gameIsPaused)
        //{
        //    menu.Resume();
        //}
        //else
        //{
        //    menu.Pause();
        //}
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Interact");
        //Check navn, lav switch
        //Tag interactable
        playerInteract.Interact();
    }

}
