using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    public float speed = 10;
    Vector3 startpos;
    Rigidbody rigidbody;
    int dir = 1;
    // Start is called before the first frame update
    void Start()
    {
        startpos = this.transform.position;
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.transform.position.x > startpos.x + 10f && dir == 1) dir = -1;
        else if (this.transform.position.x < startpos.x && dir == -1) dir = 1;
        rigidbody.velocity = new Vector3(speed, 0, 0) * dir;
    }
}
