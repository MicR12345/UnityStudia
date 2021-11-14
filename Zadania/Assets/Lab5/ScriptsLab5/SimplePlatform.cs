using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlatform : MonoBehaviour
{
    public Vector2 EndPosition;
    private Vector3 endPosV3;
    private Vector3 StartPostion;

    Rigidbody rigidbody;

    public float speed = 1f;
    bool startedMoving = false;
    bool reverseMove = false;

    private Transform OldParent;

    void Start()
    {
        StartPostion = this.transform.position;
        rigidbody = this.GetComponent<Rigidbody>();

        endPosV3 = new Vector3(EndPosition.x, StartPostion.y, EndPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        if(startedMoving && !reverseMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosV3, speed * Time.deltaTime);
        }
        if (startedMoving && reverseMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, StartPostion, speed * Time.deltaTime);
        }
        if (transform.position == endPosV3)
        {
            reverseMove = true;
        }
        if (transform.position == StartPostion && reverseMove)
        {
            reverseMove = false;
            startedMoving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered");
            OldParent = other.gameObject.transform.parent;
            other.gameObject.transform.parent = this.transform;
            if (!startedMoving && !reverseMove)
            {
                startedMoving = true;  
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Left");
            other.gameObject.transform.parent = OldParent;
        }
    }
}
