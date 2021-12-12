using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageIncreaseScript : MonoBehaviour
{
    float updateTime = 0.25f;
    float lastUpdate = 0;
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
                    wm.playerData.attackDamage = wm.playerData.attackDamage*1.2f;
                    wm.playerData.ADincreasedCounter++;
                    wm.UpdateUIs();
                    AudioSource.PlayClipAtPoint(wm.audioClips[9], this.transform.position);
                    this.gameObject.SetActive(false);
                }
            }
            lastUpdate = updateTime;
        }
        else lastUpdate = lastUpdate - Time.deltaTime;
    }
}
