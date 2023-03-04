using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{
    public Menu menu;
    public GridMovement gridMovement;
    public GameManager gameManager;
    public PlayerInputActions playerInputActions;
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    private Rigidbody2D rb;

    public float moveSpeed = 1f;
    public float tempMoveSpeed;
    [SerializeField] float collisionOffset = 0.05f;
    private ContactFilter2D movementFilter;

    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public float lastXInput;
    [HideInInspector] public float lastYInput;

    private InputAction interact;
    private InputAction pause;
    private InputAction jump;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();


    private PlayerInteract playerInteract;
    public bool specialMovement;
    public bool gridMovementBool;
    private bool canJump;


    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        
    }

    #region Enable + Disable
    private void OnEnable()
    {
        playerInputActions.Enable();
        

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
        playerInputActions.Disable();
        pause.Disable();
        interact.Disable();
        jump.Disable();
    }
    #endregion
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInteract = GetComponent<PlayerInteract>();

        specialMovement = false;
        gridMovementBool = false;
        canJump = false;
        tempMoveSpeed = moveSpeed;

        playerInputActions.Player.GridMove.performed += ctx => GridMove(ctx.ReadValue<Vector2>());
    }


    private void GridMove(Vector2 direction)
    {
        if (true)
        {
            Debug.Log("HEJ Eigil");
        }

        if (CanGridMove(direction))
        {
            transform.position += (Vector3)direction;
        }
    }

    private bool CanGridMove(Vector2 direction)
    {
        Vector3Int gridPosistion = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        return true;
        //if (!groundTilemap.HasTile(gridPosistion) || collisionTilemap.HasTile(gridPosistion))
        //{

        //    return false;
        //}
        //else 
        //{
        //    return true;
        //}
    }


    private void FixedUpdate()
    {
        if (specialMovement && gridMovementBool)
        {
            Debug.LogError("Cant have both special and grid movement on, on the same time");
        }

        if (!gridMovementBool)
        {
            //NormalMovement();
        }
        else
        {
            Debug.Log("GridMov");

        }
        


    }


    #region Movement

    private void NormalMovement()
    {
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

            if (!specialMovement && !gridMovementBool)
            {
                //normal movement.
                animator.SetBool("isMoving", success);
            }
            else if (specialMovement || gridMovementBool)
            {
                //Lav om til goat animation
                animator.SetBool("isMoving", success);
            }

        }
        else
        {
            if (!specialMovement && !gridMovementBool)
            {
                //normal movement
                animator.SetBool("isMoving", false);
            }
            else if (specialMovement || gridMovementBool)
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
            animator.SetBool("onMount", true);
            canJump = true;
            tempMoveSpeed = moveSpeed * 1.5f; //Sæt speed for movement med ged
            //Ændre animation
        }
        else if (!active)
        {
            Debug.Log("Special mov off");
            specialMovement = false;
            animator.SetBool("onMount", false);
            canJump = false;
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
        if (canJump)
        {
            Debug.Log("Jumped");
            //Check if collideren er jumpable, hvis ja, så jump animation når man går ind i den. Find ud af hvad der sker når den går ned.
            
        }
        else if (!canJump)
        {
            Debug.Log("Cant jump");
        }
        
        
    }
    private void Interact(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Interact");
        //Check navn, lav switch
        //Tag interactable
        playerInteract.Interact();
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


}
