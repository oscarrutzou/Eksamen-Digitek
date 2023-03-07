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
    [SerializeField] public int currentLane = 1; //0 = venstre, 1 = midt, 2 = højre

    [SerializeField] private Vector2 autoDirection;

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;

    //Fjern SerializeField efter test
    public bool TempLeft = false;
    public bool TempRight = false;
    [SerializeField] private Vector2 finalDirection;


    public bool Died = false;

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

        //normalMovement = true;
        //mountMovement = false;
        //gridMovement = false;
        //canAutoMoveBool = false;
        //canJump = false;
        tempMoveSpeed = moveSpeed;

        
        playerInputActions.Player.GridMove.performed += ctx => GridMove(ctx.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        if (normalMovement && gridMovement)
        {
            //Debug.LogError("Cant have both special and grid movement on, on the same time");
        }

        if (normalMovement && !gridMovement || mountMovement && !gridMovement)
        {
            NormalMovement();
        }
        else if (gridMovement && canAutoMoveBool)
        {
            //Debug.Log("GridMov");
            AutoGridMove();
        }
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


    #region Grid Movement - 3 lanes
    private void GridMove(Vector2 direction)
    {
        if (gridMovement)
        {
            if (CanGridMove(direction))
            {
                //Debug.Log("Direction + " + direction);
                //Direction er præcis 1 og virker derfor med tilemap som også er 1.
                if (TempRight)
                {
                    Debug.Log("TempRight");
  
                    transform.position += new Vector3(0, -direction.x, 0);
                }
                else if (TempLeft)
                {
                    Debug.Log("TempLeft");
                    transform.position += new Vector3(0, direction.x, 0);
                } 
                else
                {
                    Debug.Log("NOnSOS");

                    transform.position += (Vector3)direction;
                }

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
    public void GridMovement(bool active)
    {
        if (active)
        {
            if (normalMovement || mountMovement)
            {
                normalMovement = false;
                mountMovement = false;
            }
            Debug.Log("GridMovement mov on");
            animator.SetBool("onMount", true);

            gridMovement = true;
            canAutoMoveBool = true;
        }
        else if (!active)
        {
            Debug.Log("GridMovement mov off");
            //Alle grid bool = false
            animator.SetBool("onMount", false);

            gridMovement = false;
            canAutoMoveBool = false;
            TempLeft = false;
            TempRight = false;
        }
    }
    private bool CanGridMove(Vector2 direction)
    {
        Vector3Int gridPosistion = groundTilemap.WorldToCell(transform.position + (Vector3)direction);

        if (!groundTilemap.HasTile(gridPosistion))
        {
            return false;
        }

        if (!collisionTilemap.HasTile(gridPosistion))
        {
            //Sørg for at man ikke kan ændre lane ud over de 3 der er sat.
            if (direction.x < 0)
            {
                //Debug.Log("direction.x < 0: " + direction.x);
                if (currentLane == 0)
                {
                    //Debug.Log("Cant go more left");
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
                //Debug.Log("direction.x > 0: " + direction.x);
                if (currentLane == 2)
                {
                    //Debug.Log("Cant go more right");
                    return false;
                }
                else if (currentLane == 0 || currentLane == 1)
                {
                    currentLane += 1;
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region Auto Grid Movement
    private void AutoGridMove()
    {
        //AutoDirection har en direction på y, for at teste i spillet.
        //Jeg tager derfor og bruger den værdi som står der i starten til at finde hastigheden for directionen.
        //Vector2 finalDirection;

        if (canAutoMoveBool)
        {
            if (TempLeft)
            {
                //Move left
                finalDirection = new Vector2(-autoDirection.y, 0);
            }
            else if (TempRight)
            {
                //Move right
                finalDirection = new Vector2(autoDirection.y, 0);
            }
            else 
            {
                //Forward
                finalDirection = autoDirection;
            }

            //rb.MovePosition(rb.position + finalDirection * tempMoveSpeed * Time.fixedDeltaTime);

            if (!Died)
            {
                if (CanAutoGridMove(finalDirection))
                {
                    rb.MovePosition(rb.position + finalDirection * tempMoveSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
    public bool ChangeAutoMoveDirection(bool left, bool forward, bool right)
    {
        if (left)
        {
            TempLeft = true;
        }
        else if (forward)
        {
            TempLeft = false;
            TempRight = false;
        }
        else if (right)
        {
            TempRight = true;
        }

        return false;
    }
    private bool CanAutoGridMove(Vector2 direction)
    {
        //Gør det i Int, da tingene ligger på et tilemap som er et grid af 1x1 firkanter.
        Vector3Int gridPosistion = groundTilemap.WorldToCell(transform.position + (Vector3)direction);

        //For en sikkerheds skyld.
        if (!groundTilemap.HasTile(gridPosistion))
        {
            return false;
        }
        
        //Den som sørger for at tjekke om man dør når man rammer ind i noget.
        if (collisionTilemap.HasTile(gridPosistion))
        {
            Debug.Log("Player Died by slamming your head into a wall"); //Siden at personen ramte noget
            Died = true;
            return false;
        }

        return true;
    }
    #endregion

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
    }
    #endregion

    #region Mount Movement
    public void MountMovement(bool active)
    {
        if (active)
        {
            Debug.Log("Mount mov on");
            normalMovement = false;
            mountMovement = true;
            animator.SetBool("onMount", true);
            canJump = true;
            tempMoveSpeed = moveSpeed * 1.5f; //Sæt speed for movement med ged
        }
        else if (!active)
        {
            
            Debug.Log("Mount mov off");
            mountMovement = false;
            //normalMovement = true;
            animator.SetBool("onMount", false);
            canJump = false;
            tempMoveSpeed = moveSpeed;
        }
    }

    //Skal tjekke om collideren er jumpable. Måske bare slet.
    //Kan lave pop up, som siger at man skal finde 1 ting ved at gå over stenen med hesten.
    private bool CanMountJump()
    {
        return true;
    }
    #endregion

    #region Input Actions
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (canJump && mountMovement)
        {
            if (CanMountJump())
            {

            }
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
    #endregion

}
