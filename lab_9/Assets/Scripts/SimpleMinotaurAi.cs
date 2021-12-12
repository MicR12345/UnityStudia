using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMinotaurAi : MonoBehaviour
{
    public PlayerSearcher playerSearcher;
    public Rigidbody2D rigidbody2;
    public CharacterController2D controller;
    public Animator animator;

    public Vector2 start;
    public Vector2 end;

    Vector2 playerLastSeen;

    public float Speed;
    bool isReturning = false;
    public float WaitTime;
    float waitTimer = 0;
    void Update()
    {
        if (playerSearcher.PlayerCollider!=null)
        {
            waitTimer = WaitTime;
            playerLastSeen = playerSearcher.PlayerCollider.transform.position;
            if (this.transform.position.x>playerSearcher.PlayerCollider.transform.position.x)
            {
                if (this.transform.position.x>Mathf.Min(start.x,end.x))
                {
                    controller.Move(-Speed * Time.fixedDeltaTime, false);
                    animator.SetFloat("Speed",Speed);
                }
                else
                {
                    animator.SetFloat("Speed", 0);
                }
            }
            else if (this.transform.position.x < playerSearcher.PlayerCollider.transform.position.x)
            {
                if (this.transform.position.x < Mathf.Max(start.x, end.x))
                {
                    controller.Move(Speed * Time.fixedDeltaTime, false);
                    animator.SetFloat("Speed", Speed);
                }
                else
                {
                    animator.SetFloat("Speed", 0);
                }
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
        }
        else if (waitTimer>0)
        {
            waitTimer = waitTimer - Time.deltaTime;
            if (this.transform.position.x > playerLastSeen.x)
            {
                if (this.transform.position.x > Mathf.Min(start.x, end.x))
                {
                    controller.Move(-Speed * Time.fixedDeltaTime, false);
                    animator.SetFloat("Speed", Speed);
                }
                else
                {
                    animator.SetFloat("Speed", 0);
                }
            }
            else if (this.transform.position.x < playerLastSeen.x)
            {
                if (this.transform.position.x < Mathf.Max(start.x, end.x))
                {
                    controller.Move(Speed * Time.fixedDeltaTime, false);
                    animator.SetFloat("Speed", Speed);
                }
                else
                {
                    animator.SetFloat("Speed", 0);
                }
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
        }
        else
        {
            if (this.transform.position.x>=Mathf.Max(start.x, end.x) && !isReturning)
            {
                isReturning = true;
            }
            if (this.transform.position.x <= Mathf.Min(start.x, end.x) && isReturning)
            {
                isReturning = false;
            }
            if (!isReturning)
            {
                if (this.transform.position.x<Mathf.Max(start.x,end.x))
                {
                    controller.Move(Speed * Time.fixedDeltaTime, false);
                    animator.SetFloat("Speed", Speed);
                }
            }
            else
            {
                if (this.transform.position.x > Mathf.Min(start.x, end.x))
                {
                    controller.Move(-Speed * Time.fixedDeltaTime, false);
                    animator.SetFloat("Speed", Speed);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, start);
        Gizmos.DrawLine(this.transform.position, end);
    }
}
