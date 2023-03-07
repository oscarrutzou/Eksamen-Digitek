using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject playerObject;

    private Collider2D colliderTrigger;
    [SerializeField] private bool startRun = false; //player done quest
    [SerializeField] private bool stopRun = false;

    private void Start()
    {
        colliderTrigger = GetComponent<Collider2D>();
    }
    //Start & stop, grid mov trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
            playerController = collision.GetComponent<PlayerController>();

            //Debug.Log("TEst");

            if (playerController.mountMovement && startRun)
            {
                gameManager.StopMountMovement();
                gameManager.StartGridMovement();
                ///Player cutscene, efter cutscene s�t players transform til noget bestemt

            }

            if (playerController.gridMovement && stopRun)
            {
                gameManager.StopGridMovement();
                playerController.normalMovement = true;
                colliderTrigger.isTrigger = false;
            }

            //if (playerController.mountMovement)
            //{
            //    //playerController.normalMovement = false;
            //    //playerController.mountMovement = false;
            //    gameManager.StartGridMovement();

        }
    }

}
