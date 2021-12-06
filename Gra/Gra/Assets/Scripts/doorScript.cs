using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorScript : MonoBehaviour
{
    float updateTime = 0.25f;
    float lastUpdate = 0;
    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            WorldManager wm = playerMovement.worldHandler.GetComponent<WorldManager>();
            wm.MoveToNextRoom();
        }
    }*/
    private void Update()
    {
        if (lastUpdate <= 0)
        {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(new Vector2(this.transform.position.x, this.transform.position.y), 0.5f);
            foreach (Collider2D item in collider2Ds)
            {
                if (item.CompareTag("Player"))
                {
                    PlayerMovement playerMovement = item.GetComponent<PlayerMovement>();
                    WorldManager wm = playerMovement.worldHandler.GetComponent<WorldManager>();
                    wm.MoveToNextRoom();
                    break;
                }
            }
            lastUpdate = updateTime;
        }
        else lastUpdate = lastUpdate - Time.deltaTime;
    }
}
