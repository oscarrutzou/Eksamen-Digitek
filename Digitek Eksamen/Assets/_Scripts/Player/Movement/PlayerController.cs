using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Menu menu;
    //public DialogueManager dialogueManager;
    public GameManager gameManager;
    public PlayerInputActions playerInputActions;
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    private Rigidbody2D rb;

    [Header("Normal Movement")]
    public float moveSpeed = 1f;
    public float tempMoveSpeed;
    [SerializeField] private float collisionOffset = 0.05f;
    private ContactFilter2D movementFilter;

    private Vector2 movementInput;
    //private Vector2 gridMovementInput;

    [Header("On Mount")]
    [SerializeField] private float onMountSpeed;
    [SerializeField] private float jumpForce = 2f;

    private float lastXInput;
    private float lastYInput;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private PlayerInteract playerInteract;

    [Header("Bools Movement")]
    public bool normalMovement;
    public bool mountMovement;
    public bool gridMovement;
    //public bool canJump;
    public bool isJumping;


    [Header("Grid Movement")]
    [SerializeField] private bool canAutoMoveBool = false;
    [SerializeField] public int currentLane = 1; //0 = venstre, 1 = midt, 2 = højre

    [SerializeField] private Vector2 autoDirection;

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;

    public bool GridMovAutoLeft = false;
    public bool GridMovAutoRight = false; //Måske ændre til GridMovAutoLeft
    private Vector2 finalDirection;

    public bool Died = false;

    private static PlayerController instance;

    #region GetInstance til andre scripts
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    public static PlayerController GetInstance()
    {
        return instance;
    }
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInteract = GetComponent<PlayerInteract>();

        normalMovement = true;
        mountMovement = false;
        gridMovement = false;
        canAutoMoveBool = false;
        //canJump = false;
        tempMoveSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        //Inputs before movement
        if (InputManager.GetInstance().GetPausePressed())
        {
            PauseMenuUI();
        } //Pause
        if (InputManager.GetInstance().GetInteractPressed())
        {
            Interact();
        } //Interact

        if (normalMovement && !gridMovement || mountMovement && !gridMovement)
        {
            movementInput = InputManager.GetInstance().GetMoveDirection();
            NormalMovement(movementInput);
        }
        else if (gridMovement && canAutoMoveBool)
        {
            AutoGridMove();
        }
    }

    #region Grid Movement - 3 lanes
    public void HandleGridMove(Vector2 direction)
    {
        //!dialogueManager.dialogIsActive
        if (gridMovement)
        {
            if (CanGridMove(direction))
            {
                //Direction er præcis 1 og virker derfor med tilemap som også er 1.
                if (GridMovAutoRight)
                {
                    transform.position += new Vector3(0, -direction.x, 0);
                }
                else if (GridMovAutoLeft)
                {
                    transform.position += new Vector3(0, direction.x, 0);
                }
                else
                {
                    transform.position += (Vector3)direction;
                }
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
            GridMovAutoLeft = false;
            GridMovAutoRight = false;
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
                if (currentLane == 0)
                {
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
            if (GridMovAutoLeft)
            {
                //Move left
                finalDirection = new Vector2(-autoDirection.y, 0);
            }
            else if (GridMovAutoRight)
            {
                //Move right
                finalDirection = new Vector2(autoDirection.y, 0);
            }
            else
            {
                //Forward
                finalDirection = autoDirection;
            }

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
            GridMovAutoLeft = true;
        }
        else if (forward)
        {
            GridMovAutoLeft = false;
            GridMovAutoRight = false;
        }
        else if (right)
        {
            GridMovAutoRight = true;
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
    private void NormalMovement(Vector2 direction)
    {
        //normalMovement && !dialogueManager.dialogIsActive || mountMovement && !dialogueManager.dialogIsActive
        if (normalMovement || mountMovement)
        {

            #region Movement Input
            if (direction != Vector2.zero)
            {
                bool success = TryMove(direction);

                if (!success)
                {
                    success = TryMove(new Vector2(direction.x, 0));
                }

                if (!success)
                {
                    success = TryMove(new Vector2(0, direction.y));
                }

                if (normalMovement || mountMovement || gridMovement)
                {
                    //Lav om til goat animation
                    animator.SetBool("isMoving", success);
                    animator.SetFloat("horizontalMovement", direction.x);
                    animator.SetFloat("verticalMovement", direction.y);
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
            #endregion
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                    direction,
                    movementFilter,
                    castCollisions,
                    moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * tempMoveSpeed * Time.fixedDeltaTime);
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

    #endregion

    #region OnMount 
    public void MountMovement(bool active)
    {
        if (active)
        {
            Debug.Log("Mount mov on");
            normalMovement = false;
            mountMovement = true;
            animator.SetBool("onMount", true);

            //canJump = true;
            tempMoveSpeed = onMountSpeed; 
        }
        else if (!active)
        {

            Debug.Log("Mount mov off");
            mountMovement = false;
            //normalMovement = true;
            animator.SetBool("onMount", false);
            //canJump = false;
            tempMoveSpeed = moveSpeed;
        }
    }

    private void MountJump()
    {
        if (isJumping)
        {
            return;
        }
        else if (!isJumping)
        {
           StartCoroutine(WaitForJump());
        }
    }

    private IEnumerator WaitForJump()
    {
        isJumping = true;
        Vector2 jumpDirection = new Vector2(0, jumpForce);
        rb.AddForce(jumpDirection, ForceMode2D.Impulse);

        yield return new WaitForSecondsRealtime(1f);
        isJumping = false;
    }

    #endregion

    #region Misc Input Metoder
    private void Interact()
    {
        //Debug.Log("Interact");

        playerInteract.Interact();
    }
    private void PauseMenuUI()
    {
        //Debug.Log("Pause menu open");
        //Pause UI menu
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
