using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AiTargetGen : MonoBehaviour
{
    public Transform target;
    Transform targetObject;

    public Vector3 followOffset;
    public float followAboveDistance;

    public LayerMask countedAsObstructions;
    public LayerMask targetMask;

    public AIDestinationSetter destinationSetter;
    public PlayerSearcher searcher;
    void Start()
    {
        targetObject = new GameObject(this.gameObject.name + " pathfinding target").transform;
        destinationSetter.target = targetObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (searcher.PlayerCollider != null)
        {
            Vector3 reversedFollowOffset = new Vector3(-followOffset.x, followOffset.y, followOffset.z);
            if (Mathf.Abs(Vector3.Distance(this.transform.position, (target.position + followOffset))) <= Mathf.Abs(Vector3.Distance(this.transform.position, (target.position + reversedFollowOffset))))
            {
                targetObject.position = target.position + followOffset;
                Vector3 dir = (targetObject.transform.position - target.transform.position).normalized;
                RaycastHit2D raycastHit2 = Physics2D.Raycast(targetObject.position, dir, followOffset.x, countedAsObstructions + targetMask);
                if (raycastHit2.collider != null && !raycastHit2.collider.CompareTag(target.tag))
                {

                    targetObject.position = target.position + reversedFollowOffset;
                    Vector3 dir2 = (targetObject.transform.position - target.transform.position).normalized;
                    RaycastHit2D raycastHit22 = Physics2D.Raycast(targetObject.position, dir2, followOffset.x, countedAsObstructions + targetMask);
                    if (raycastHit22.collider != null && !raycastHit22.collider.CompareTag(target.tag))
                    {
                        targetObject.position = target.position + new Vector3(0, followAboveDistance, 0);
                    }

                }
            }
            else
            {
                targetObject.position = target.position + reversedFollowOffset;
                Vector3 dir2 = (targetObject.transform.position - target.transform.position).normalized;
                RaycastHit2D raycastHit22 = Physics2D.Raycast(targetObject.position, dir2, followOffset.x, countedAsObstructions + targetMask);
                if (raycastHit22.collider != null && !raycastHit22.collider.CompareTag(target.tag))
                {
                    targetObject.position = target.position + followOffset;
                    Vector3 dir = (targetObject.transform.position - target.transform.position).normalized;
                    RaycastHit2D raycastHit2 = Physics2D.Raycast(targetObject.position, dir, followOffset.x, countedAsObstructions + targetMask);

                    if (raycastHit2.collider != null && !raycastHit2.collider.CompareTag(target.tag))
                    {
                        targetObject.position = target.position + new Vector3(0, followAboveDistance, 0);
                    }

                }
            }

        }
        else
        {
            targetObject.position = this.transform.position;
        }
    }
}
