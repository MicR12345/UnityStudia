using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerSearcher : MonoBehaviour
{
    public float PlayerSearchTime;
    public float SearchRadius;
    public LayerMask SearchForThoseColliders;
    public LayerMask RaycastColliders;
    public string PlayerTag;
    //[HideInInspector]
    public Collider2D PlayerCollider;
    float timer = 0;
    void Update()
    {
        UpdateTimer();
        if (timer <= 0)
        {
            Collider2D[] colliders2D = Physics2D.OverlapCircleAll(this.transform.position, SearchRadius,SearchForThoseColliders);
            foreach (Collider2D item in colliders2D)
            {
                if (item.CompareTag(PlayerTag))
                {
                    Vector3 dir = (item.transform.position - this.transform.position).normalized;
                     RaycastHit2D tryRaycasting = Physics2D.Raycast(this.transform.position, dir, SearchRadius, RaycastColliders);
                    if(tryRaycasting.collider!=null && tryRaycasting.collider.CompareTag(PlayerTag)) Debug.DrawLine(this.transform.position, tryRaycasting.transform.position,Color.black,PlayerSearchTime);
                    if (tryRaycasting.collider!=null && tryRaycasting.collider.CompareTag(PlayerTag))
                    {
                        PlayerCollider = item;
                        break;
                    }
                    else
                    {
                        PlayerCollider = null;
                    }
                }
                else
                {
                    PlayerCollider = null;
                }
            }
            if (colliders2D.Length == 0)
            {
                PlayerCollider = null;
            }
            {

            }
            timer = PlayerSearchTime;
        }
    }

    void UpdateTimer()
    {
        if (timer > 0) timer = timer - Time.deltaTime;
    }
}
