using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;

    [SerializeField]
    private Collider2D[] interactColliderArray;

    public void Interact()
    {
        interactColliderArray = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (Collider2D collider2D in interactColliderArray)
        {
            Interactable interactable = collider2D.GetComponent<Interactable>();

            Debug.Log(interactable);
            if (interactable != null)
            {
                switch (interactable)
                {
                    case NPC npc:
                        npc.NPCInteract();
                        break;

                    //For f�rste quest
                    case GoatInteractable goat:
                        goat.ObjectInteract();
                        break;
                    case ShoeInteractable shoe:
                        shoe.ObjectInteract();
                        break;
                    case MudInteractable mud:
                        mud.ObjectInteract();
                        break;
                    case CrowInteractable crow:
                        crow.ObjectInteract();
                        break;

                    // add more cases for other types of interactable objects
                    default:
                        // handle other cases here
                        Debug.LogError("Check if item du pr�ver at interact med er p� listen ved switchen");
                        break;
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(this.transform.position, interactRange);
    //}

}
