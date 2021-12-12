using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class FlipEnemyScript : MonoBehaviour
{
    public AIPath aiPath;
    public AiTargetGen targetGen;
    Transform target;
    private void Start()
    {
        target = targetGen.target;
    }
    void Update()
    {
        if (aiPath.desiredVelocity.x<0 && this.transform.localScale.x>0)
        {
            this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
        }
        else if (aiPath.desiredVelocity.x >= 0 && this.transform.localScale.x < 0)
        {
            this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
        }
        GraphNode node1 = AstarPath.active.GetNearest(this.transform.position, NNConstraint.Default).node;
        GraphNode node2 = AstarPath.active.GetNearest(target.transform.position, NNConstraint.Default).node;
        if (!PathUtilities.IsPathPossible(node1,node2))
        {
            if (this.transform.position.x - target.transform.position.x > 0)
            {
                if (this.transform.localScale.x > 0)
                {
                    this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
                }
            }
            else
            {
                if (this.transform.localScale.x < 0)
                {
                    this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
                }
            }
        }
        Debug.Log(aiPath.velocity);
        if (Mathf.Abs(aiPath.velocity.x)<=0.4)
        {
            if (this.transform.position.x-target.transform.position.x>0)
            {
                if (this.transform.localScale.x > 0)
                {
                    this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
                }
            }
            else
            {
                if (this.transform.localScale.x < 0)
                {
                    this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
                }
            }
        }
    }
}
