using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject playerSpawnPoint;

    private Collider2D colliderTrigger;
    [SerializeField] private bool startRun = false; //player done quest
    [SerializeField] private bool stopRun = false;

    [SerializeField] private GameObject visualVFXObject;

    private void Start()
    {
        colliderTrigger = GetComponent<Collider2D>();

    }

    private void Update()
    {
        //Kunne have brugt event based system
        if (startRun && colliderTrigger.isTrigger)
        {
            colliderTrigger.isTrigger = false;
        }

        if (startRun 
            && !colliderTrigger.isTrigger 
            && ((Ink.Runtime.IntValue)DialogueManager.GetInstance().GetVariableState("questItemsCollected")).value == 3 
            && PlayerController.GetInstance().mountMovement)
        {
            colliderTrigger.isTrigger = true;
            visualVFXObject.SetActive(false);
        }
    }

    //Start & stop, grid mov trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
            playerController = collision.GetComponent<PlayerController>();

            if (playerController.mountMovement && startRun)
            {
                MountToGridMovement();
            }

            if (playerController.gridMovement && stopRun)
            {
                GridToNormalMovement();
            }
        }
    }

    public void MountToGridMovement()
    {
        ///Player cutscene, efter cutscene players transform til noget bestemt
        gameManager.StopMountMovement();
        playerObject.transform.position = playerSpawnPoint.transform.position;
        gameManager.StartGridMovement();
        colliderTrigger.isTrigger = false;
        visualVFXObject.SetActive(true);

    }

    public void GridToNormalMovement()
    {
        gameManager.StopGridMovement();
        playerController.normalMovement = true;
        colliderTrigger.isTrigger = false;
        visualVFXObject.SetActive(true);

        Ink.Runtime.BoolValue newValue = new Ink.Runtime.BoolValue(true);
        DialogueManager.GetInstance().SetVariableState("hasFinnishedGridLvl", new Ink.Runtime.BoolValue(true));
    }

}
