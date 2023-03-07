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


            if (turnLeft && !playerController.TempLeft)
            {
                //Debug.Log("left");
                playerController.TempLeft = true;
                playerController.TempRight = false;
                playerController.currentLane = lane;


                playerObject.transform.position = new Vector3(playerObject.transform.position.x,
                    this.gameObject.transform.position.y - 1, playerObject.transform.position.z);
            } 
            else if (turnRight && !playerController.TempRight)
            {
                //Debug.Log("right");
                playerController.TempRight = true;
                playerController.TempLeft = false;
                playerController.currentLane = lane;

                playerObject.transform.position = new Vector3(playerObject.transform.position.x, 
                    this.gameObject.transform.position.y - 1, playerObject.transform.position.z);

            } 
            else if (turnForward)
            {
                if (playerController.TempLeft)
                {
                    playerObject.transform.position = new Vector3(this.gameObject.transform.position.x + 1,
                        playerObject.transform.position.y, playerObject.transform.position.z);

                    playerController.TempLeft = false;
                    playerController.currentLane = lane;

                }
                else if (playerController.TempRight)
                {
                    playerObject.transform.position = new Vector3(this.gameObject.transform.position.x - 1,
                        playerObject.transform.position.y, playerObject.transform.position.z);

                    playerController.TempRight = false;
                    playerController.currentLane = lane;

                }
            }
        }
    }
}
