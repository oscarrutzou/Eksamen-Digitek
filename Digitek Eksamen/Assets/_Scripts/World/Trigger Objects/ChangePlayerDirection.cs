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
    [SerializeField] private bool turnDown;

    [SerializeField] private int lane;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
            playerController = collision.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Mathf.Abs(playerObject.transform.position.x - transform.position.x) < 0.1f &&
                Mathf.Abs(playerObject.transform.position.y - transform.position.y) < 0.1f)
            {
                if (turnLeft && !playerController.GridMovAutoLeft)
                {
                    ChangeGridMov(true, false, false);
                }
                else if (turnRight && !playerController.GridMovAutoRight)
                {
                    ChangeGridMov(false, true, false);

                }
                else if (turnForward)
                { 
                    ChangeGridMov(false, false, false);

                }
                else if (turnDown && !playerController.GridMovAutoDown)
                {
                    ChangeGridMov(false, false, true);
                }
            }
        }
    }

    private void ChangeGridMov(bool Left, bool Right, bool Down)
    {
        playerController.GridMovAutoLeft = Left;
        playerController.GridMovAutoRight = Right;
        playerController.GridMovAutoDown = Down;

        playerController.currentLane = lane;

        playerObject.transform.position = new Vector3(this.gameObject.transform.position.x, playerObject.transform.position.y, playerObject.transform.position.z);
        if (Left || Right)
        {
            playerObject.transform.position = new Vector3(playerObject.transform.position.x, this.gameObject.transform.position.y, playerObject.transform.position.z);
        }
        else if (!Left && !Right && !Down || !Left && !Right && Down)
        {
            playerObject.transform.position = new Vector3(this.gameObject.transform.position.x, playerObject.transform.position.y, playerObject.transform.position.z);
        }
    }
}
