using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject playerObject;

    //Start & stop, grid mov trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
            playerController = collision.GetComponent<PlayerController>();
            
            if (playerController.mountMovement)
            {
                gameManager.StartGridMovement();
                playerController.mountMovement = false;
                
            }
            else if (playerController.gridMovement)
            {
                gameManager.StopGridMovement();
                playerController.normalMovement = true;
            }
        }
    }

}
