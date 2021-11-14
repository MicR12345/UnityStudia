using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexPlatform : MonoBehaviour
{
    public List<Vector3> PathPoints = new List<Vector3>();
    private Vector3 StartPostion;

    public float speed = 1f;

    int currentPoint = 0;

    bool startedMoving = false;
    bool reverseMove = false;

    private Transform OldParent;

    void Start()
    {
        StartPostion = this.transform.position;
        PathPoints.Insert(0, StartPostion);
    }

    // Update is called once per frame
    void Update()
    {
        if (startedMoving && !reverseMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, PathPoints[currentPoint], speed * Time.deltaTime);
        }
        if (startedMoving && reverseMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, PathPoints[currentPoint], speed * Time.deltaTime);
        }
        if (transform.position == StartPostion && reverseMove)
        {
            reverseMove = false;
            startedMoving = false;
        }
        else if (transform.position == PathPoints[currentPoint] && !reverseMove && currentPoint != PathPoints.Count - 1)
        {
            currentPoint++;
        }
        else if (transform.position == PathPoints[currentPoint] && reverseMove && currentPoint != PathPoints.Count - 1)
        {
            currentPoint--;
        }
        else if (currentPoint == PathPoints.Count-1 && transform.position == PathPoints[currentPoint])
        {
            reverseMove = true;
            currentPoint--;
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
