using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectableTrigger : MonoBehaviour
{
    [SerializeField] private string soundClipName;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Menu.GetInstance().AddCurrentCollectableScore();

            if (soundClipName != null)
            {
                AudioManager.GetInstance().Play(soundClipName);
            }
            else
            {
                Debug.LogWarning("No soundclip in gameobject: " + gameObject.name);
            }

            //Deactivate
            this.gameObject.SetActive(false);
        }
    }

}
