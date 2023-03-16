using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerDirection : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private GameObject playerObject;

    [SerializeField] private bool turnLeft;
    [SerializeField] private bool turnRight;
    [SerializeField] private bool turnForward;

    [SerializeField]
    private int lane;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
            playerController = collision.GetComponent<PlayerController>();


            if (turnLeft && !playerController.GridMovAutoLeft)
            {
                //Debug.Log("left");
                playerController.GridMovAutoLeft = true;
                playerController.GridMovAutoRight = false;
                playerController.currentLane = lane;


                playerObject.transform.position = new Vector3(playerObject.transform.position.x,
                    this.gameObject.transform.position.y - 1, playerObject.transform.position.z);
            } 
            else if (turnRight && !playerController.GridMovAutoRight)
            {
                //Debug.Log("right");
                playerController.GridMovAutoRight = true;
                playerController.GridMovAutoLeft = false;
                playerController.currentLane = lane;

                playerObject.transform.position = new Vector3(playerObject.transform.position.x, 
                    this.gameObject.transform.position.y - 1, playerObject.transform.position.z);

            } 
            else if (turnForward)
            {
                if (playerController.GridMovAutoLeft)
                {
                    playerObject.transform.position = new Vector3(this.gameObject.transform.position.x + 1,
                        playerObject.transform.position.y, playerObject.transform.position.z);

                    playerController.GridMovAutoLeft = false;
                    playerController.currentLane = lane;

                }
                else if (playerController.GridMovAutoRight)
                {
                    playerObject.transform.position = new Vector3(this.gameObject.transform.position.x - 1,
                        playerObject.transform.position.y, playerObject.transform.position.z);

                    playerController.GridMovAutoRight = false;
                    playerController.currentLane = lane;

                }
            }
        }
    }
}
