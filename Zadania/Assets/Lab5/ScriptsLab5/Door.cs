using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject door;

    private Vector3 StartPosition;
    private Vector3 OpenPosition;
    private float zMoveDistance = 3f;

    MeshFilter meshFilter;

    public float speed = 1f;

    bool isTherePlayer = false;
    // Start is called before the first frame update
    private void Start()
    {
        StartPosition = door.transform.position;
        meshFilter = this.GetComponent<MeshFilter>();
        zMoveDistance = meshFilter.mesh.bounds.size.x;
        OpenPosition = new Vector3(StartPosition.x + zMoveDistance, StartPosition.y, StartPosition.z);

        
        Debug.Log(zMoveDistance);
    }
    private void Update()
    {
        if (isTherePlayer)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, OpenPosition, speed * Time.deltaTime);
        }
        else
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, StartPosition, speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Open");
            isTherePlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Close");
            isTherePlayer = false;
        }
    }
}
