using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectableTrigger : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Menu.GetInstance().AddCurrentCollectableScore();
            //Deactivate
            this.gameObject.SetActive(false);
        }
    }

}
