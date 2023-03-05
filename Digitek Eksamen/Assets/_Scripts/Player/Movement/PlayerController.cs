using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Menu menu;
    public GameManager gameManager;
    public PlayerInputActions playerInputActions;
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    private Rigidbody2D rb;

    [Header("Normal Movement")]
    public float moveSpeed = 1f;
    public float tempMoveSpeed;
    
    [SerializeField] float collisionOffset = 0.05f;
    private ContactFilter2D movementFilter;

    private Vector2 movementInput;
    private float lastXInput;
    private float lastYInput;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    [HideInInspector]
    [Header("InputActions")]
    private PlayerInteract playerInteract;
    private InputAction interact;
    private InputAction pause;
    private InputAction jump;


    [Header("Bools Movement")]
    public bool normalMovement;
    public bool mountMovement;
    public bool gridMovement;

    private bool canJump; //Hører til mount movement

    [Header("Grid Movement")]
    [SerializeField] private bool canAutoMoveBool = false;
    [SerializeField] private int currentLane = 1; //0 = venstre, 1 = midt, 2 = højre

    [SerializeField] private Vector2 autoDirection;

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

        //normalMovement = true;
        mountMovement = false;
        gridMovement = false;
        canAutoMoveBool = false;
        canJump = false;
        tempMoveSpeed = moveSpeed;

        
        playerInputActions.Player.GridMove.performed += ctx => GridMove(ctx.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        if (normalMovement && gridMovement)
        {
            //Debug.LogError("Cant have both special and grid movement on, on the same time");
        }

        if (!gridMovement)
        {
            //NormalMovement();
        }
        else if (gridMovement && canAutoMoveBool)
        {
            Debug.Log("GridMov");
            rb.MovePosition(rb.position + autoDirection * tempMoveSpeed * Time.fixedDeltaTime);
        }
    }


    private void AutoGridMove()
    {
        Debug.Log("GridMov");
        if (canAutoMoveBool)
        {
            //Find direction
            //can move? hvis ikke, se om det er en collider og spilleren taber

        }
    }

    private void GridMove(Vector2 direction)
    {
        if (gridMovement)
        {
            if (CanGridMove(direction))
            {
                Debug.Log("Direction + " + direction);
                //Direction er præcis 1 og virker derfor med tilemap som også er 1.
                transform.position += (Vector3)direction;

                #region Flip Sprite
                if (direction.x != 0)
                {
                    lastXInput = direction.x;
                }
                else if (movementInput.y != 0)
                {
                    lastYInput = direction.y;
                }

                if (direction.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else if (direction.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                #endregion
            }
        }
    }
    private bool CanGridMove(Vector2 direction)
    {
        Vector3Int gridPosistion = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        //if (!groundTilemap.HasTile(gridPosistion) || collisionTilemap.HasTile(gridPosistion))
        //{

        //    return false;
        //}
        //else 
        //{
        //    return true;
        //}

        //if (collisionTilemap.HasTile(gridPosistion))
        //{
        //    Debug.Log("Player Died"); //Siden at personen ramte noget
        //}

        //Sørg for at man ikke kan ændre lane ud over de 3 der er sat.
        if (direction.x < 0)
        {
            if (currentLane == 0)
            {
                Debug.Log("Cant go more left" + direction.x);
                return false;
            }
            else if (currentLane == 1 || currentLane == 2)
            {
                currentLane -= 1;
                return true;
            }
        } 
        else if (direction.x > 0)
        {
            if (currentLane == 2)
            {
                Debug.Log("Cant go more right" + direction.x);
                return false;
            }
            else if (currentLane == 0 || currentLane == 1)
            {
                currentLane += 1;
                return true;
            }
        }

        //Skal skrive dette for at den kan virke
        if (true)
        {
            return false;
        }
    }





    #region Normal Movement
    private void NormalMovement()
    {
        if (normalMovement || mountMovement)
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

                if (!mountMovement && !gridMovement)
                {
                    //normal movement.
                    animator.SetBool("isMoving", success);
                }
                else if (mountMovement || gridMovement)
                {
                    //Lav om til goat animation
                    animator.SetBool("isMoving", success);
                }

            }
            else
            {
                if (!mountMovement && !gridMovement)
                {
                    //normal movement
                    animator.SetBool("isMoving", false);
                }
                else if (mountMovement || gridMovement)
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
        //Debug.Log("Moveminput: " + movementInput);
    }
    #endregion

    //Ged movement
    public void MountMovement(bool active)
    {
        if (active)
        {
            Debug.Log("Special mov on");
            mountMovement = true;
            animator.SetBool("onMount", true);
            canJump = true;
            tempMoveSpeed = moveSpeed * 1.5f; //Sæt speed for movement med ged
            //Ændre animation
        }
        else if (!active)
        {
            Debug.Log("Special mov off");
            mountMovement = false;
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

        gameManager.StopMountMovement();
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
