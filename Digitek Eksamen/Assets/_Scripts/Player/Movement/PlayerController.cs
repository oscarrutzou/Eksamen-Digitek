using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Animator animator;
    private Rigidbody2D rb;

    [Header("Speed")]
    public float moveSpeed = 1f;
    [SerializeField] private float onMountSpeed;
    [SerializeField] private float autoMoveSpeed;
    public float tempMoveSpeed;

    [Header("Normal Movement")]
    [SerializeField] private float collisionOffset = 0.05f;
    private ContactFilter2D movementFilter;
    private Vector2 movementInput;

    [Header("On Mount")]
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
    [SerializeField] public int currentLane = 1; //0 = venstre, 1 = midt, 2 = h�jre

    [SerializeField] private Vector2 autoDirection;

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private Tilemap inFrontPlayerTilemap;
    [SerializeField] private Tilemap behindPlayerTilemap;

    public bool GridMovAutoLeft = false;
    public bool GridMovAutoRight = false; //M�ske �ndre til GridMovAutoLeft
    public bool GridMovAutoDown = false;
    private Vector2 finalDirection;

    public bool Died = false;

    [DoNotSerialize] public bool isAllowedToMove = true;

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
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInteract = GetComponent<PlayerInteract>();

        normalMovement = true;
        mountMovement = false;
        gridMovement = false;
        canAutoMoveBool = false;
        //canJump = false;
        tempMoveSpeed = moveSpeed;


        isAllowedToMove = GameManager.GetInstance().playerIsAllowedToMove;
    }

    private void FixedUpdate()
    {
        if (Died) return;

        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        //Inputs before movement
        if (InputManager.GetInstance().GetPausePressed())
        {
            menu.PauseMenu();
        } //Pause
        if (InputManager.GetInstance().GetInteractPressed())
        {
            Interact();
        } //Interact

        if (!isAllowedToMove)
        {
            animator.SetBool("isMoving", false);
            return;
         }

        //Movement hvis man ikke er i gridmovement.
        if (normalMovement && !gridMovement || mountMovement && !gridMovement)
        {
            //F�r movementInput direction fra inputmanager.
            movementInput = InputManager.GetInstance().GetMoveDirection();
            //Funktion til at teste om man kan flytte sig den retning.
            //Hvis man ikke kan, s� bev�ger man sig ikke.
            NormalMovement(movementInput); 
        }
        //Laver grid movement hvis gridmovement er true.
        else if (gridMovement && canAutoMoveBool) 
        {
            //S�tter en konstant movement speed og flytter spilleren.
            AutoGridMove();
        }
    }



    #region Grid Movement - 3 lanes
    public void HandleGridMove(Vector2 direction)
    {
        if (Died) return;

        //!dialogueManager.dialogIsActive
        if (gridMovement)
        {
            if (CanGridMove(direction))
            {
                //Direction er pr�cis 1 og virker derfor med tilemap som ogs� er 1.
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
            tempMoveSpeed = autoMoveSpeed;
            animator.SetBool("onMount", true);
            animator.SetBool("isMoving", true);
            currentLane = 1;

            gridMovement = true;
            canAutoMoveBool = true;
            GridMovAutoRight = true;
            GridMovAutoLeft = false;
            GridMovAutoDown = false;
        }
        else if (!active)
        {
            tempMoveSpeed = moveSpeed;
            animator.SetBool("onMount", false);

            gridMovement = false;
            canAutoMoveBool = false;
            GridMovAutoLeft = false;
            GridMovAutoRight = false;
            GridMovAutoDown = false;
        }
    }
    private bool CanGridMove(Vector2 direction)
    {
        Vector3Int gridPosistionStraight = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        Vector3Int gridPosistionRight = groundTilemap.WorldToCell(transform.position + new Vector3(0, -direction.x, 0));
        Vector3Int gridPosistionLeft = groundTilemap.WorldToCell(transform.position + new Vector3(0, direction.x, 0));

        if (GridMovAutoRight)
        {
            if (!groundTilemap.HasTile(gridPosistionRight))
            {
                return false;
            }

            if (!collisionTilemap.HasTile(gridPosistionRight))
            {
                //S�rg for at man ikke kan �ndre lane ud over de 3 der er sat.
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
        }
        else if (GridMovAutoLeft)
        {
            if (!groundTilemap.HasTile(gridPosistionLeft))
            {
                return false;
            }

            if (!collisionTilemap.HasTile(gridPosistionLeft))
            {
                //S�rg for at man ikke kan �ndre lane ud over de 3 der er sat.
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
        }
        else
        {
            if (!groundTilemap.HasTile(gridPosistionStraight))
            {
                return false;
            }

            if (!collisionTilemap.HasTile(gridPosistionStraight))
            {
                //S�rg for at man ikke kan �ndre lane ud over de 3 der er sat.
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
        }

        return false;
    }
    #endregion

    #region Auto Grid Movement
    private void AutoGridMove()
    {
        //AutoDirection har en direction p� y, for at teste i spillet.
        //Jeg tager derfor og bruger den v�rdi som st�r der i starten til at finde hastigheden for directionen.
        //Vector2 finalDirection;

        if (canAutoMoveBool)
        {
            animator.SetBool("isMoving", true);

            if (GridMovAutoLeft)
            {
                //Move left
                finalDirection = new Vector2(-autoDirection.y, 0);
                animator.SetFloat("horizontalMovement", new Vector2(-1, 0).x);
            }
            else if (GridMovAutoRight)
            {
                //Move right
                animator.SetFloat("horizontalMovement", new Vector2(1, 0).x);
                finalDirection = new Vector2(autoDirection.y, 0);
            }
            else if (GridMovAutoDown)
            {
                // Move down
                animator.SetFloat("horizontalMovement", new Vector2(0, 0).x);
                animator.SetFloat("verticalMovement", new Vector2(0, -1).y);
                finalDirection = new Vector2(0, -autoDirection.y);
            }
            else
            {
                //Forward
                animator.SetFloat("horizontalMovement", new Vector2(0, 0).x);
                animator.SetFloat("verticalMovement", new Vector2(0, 1).y);
                finalDirection = autoDirection;
            }

            if (!Died)
            {
                if (CanAutoGridMove(finalDirection))
                {
                    rb.MovePosition(rb.position + finalDirection * tempMoveSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    //Bliver kaldt da playeren har ramt ind i en v�g
                    PlayerDied();
                }
            }
        }
    }

    private bool CanAutoGridMove(Vector2 direction)
    {
        //G�r det i Int, da tingene ligger p� et tilemap som er et grid af 1x1 firkanter.
        Vector3Int gridPosistion = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        
        //For en sikkerheds skyld.
        if (!groundTilemap.HasTile(gridPosistion))
        {
            return false;
        }

        //Den som s�rger for at tjekke om man d�r n�r man rammer ind i noget.
        if (collisionTilemap.HasTile(gridPosistion))
        {
            PlayerDied();
            return false;
        }

        return true;
    }
    #endregion

    #region Normal Movement
    //Direction kommer fra InputManageren.
    private void NormalMovement(Vector2 direction)
    {
        //Bliver brugt b�de n�r man er uder eller p� sin ged.
        if (normalMovement || mountMovement) 
        {
            if (direction != Vector2.zero) //Tjek der er input.
            {
                //Tjekker om man kan flytte sig den direction man trykkede.
                bool success = TryMove(direction); 

                //F�rst tjek x direction.
                if (!success)
                {
                    success = TryMove(new Vector2(direction.x, 0));
                }

                //S� tjek y direction.
                if (!success)
                {
                    success = TryMove(new Vector2(0, direction.y));
                }

                //Flytter spilleren ved dens rigidbody.
                //Bruger fixedDeltaTime, for at g�re hastigheden ens p� alle computere.
                rb.MovePosition(rb.position + direction * tempMoveSpeed * Time.fixedDeltaTime);

                //Hvis begge er true s� kan man bev�ge sig.
                //Direction v�rdier bliver sat i animator.
                animator.SetBool("isMoving", success);
                animator.SetFloat("horizontalMovement", direction.x);
                animator.SetFloat("verticalMovement", direction.y);
            }
            else //Player er st�r stille 
            {
                if (!mountMovement && !gridMovement)
                {
                    //
                    animator.SetBool("isMoving", false);
                }
                else if (mountMovement || gridMovement)
                {

                    animator.SetBool("isMoving", false);
                }
            }
        }
    }

    //Tester om man kan bev�ge sig i en vector2 retning.
    private bool TryMove(Vector2 direction)
    {
        //Extra sikkerhed for at s�rge for man pr�ver at flytte sig
        if (direction != Vector2.zero)
        {
            //Laver et cast og ser om det er muligt at komme i den direction.
            //Kigger p� collisions og bruger de samme v�rdier som vi bruger til at flytte spilleren.
            int count = rb.Cast(
                    direction,
                    movementFilter,
                    castCollisions,
                    moveSpeed * Time.fixedDeltaTime + collisionOffset);
            
            //Hvis int er 0, betyder det at det er okay at flytte sig. 
            if (count == 0)
            {
                return true;
            }
            else //Returner false, hvis den laver en collision med noget i rb.Cast. 
            {
                return false;
            }
        }
        else //Direction er 0
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
            //Debug.Log("Mount mov on");
            normalMovement = false;
            mountMovement = true;
            animator.SetBool("onMount", true);

            //canJump = true;
            tempMoveSpeed = onMountSpeed; 
        }
        else if (!active)
        {

            //Debug.Log("Mount mov off");
            mountMovement = false;
            //normalMovement = true;
            animator.SetBool("onMount", false);
            //canJump = false;
            tempMoveSpeed = moveSpeed;
        }
    }

    #endregion

    #region Misc functions
    private void Interact()
    {
        playerInteract.Interact();
    }

    public void PlayerDied()
    {
        //D�de p� en m�de, kan blive kaldt i andre scripts.
        Died = true;
        animator.SetBool("isMoving", false);
        gameManager.PlayerDead();
    }

    #endregion
}
