using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove2 : MonoBehaviour
{
    public float speed = 10;
    public Vector3 startpos;
    Rigidbody rigidbody;
    Vector3 dir = new Vector3(1, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        startpos = this.transform.position;
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mathf.Abs(startpos.x - this.transform.position.x) > 10f || Mathf.Abs(startpos.z - this.transform.position.z) > 10f)
        {
            this.transform.Rotate(new Vector3(0, 90f, 0));
            if(this.transform.position.x > startpos.x) 
                this.transform.position = new Vector3(startpos.x + 10f, startpos.y, startpos.z);
            else if(this.transform.position.x < startpos.x) 
                this.transform.position = new Vector3(startpos.x -10f, startpos.y, startpos.z);
            else if (this.transform.position.z > startpos.z) 
                this.transform.position = new Vector3(startpos.x, startpos.y, startpos.z + 10f);
            else this.transform.position = new Vector3(startpos.x, startpos.y, startpos.z - 10f);
            startpos = new Vector3(Mathf.Floor(this.transform.position.x), this.transform.position.y, Mathf.Floor(this.transform.position.z));
            if (dir == new Vector3(1, 0, 0)) dir = new Vector3(0, 0, 1);
            else if (dir == new Vector3(0, 0, 1)) dir = new Vector3(-1, 0, 0);
            else if (dir == new Vector3(-1, 0, 0)) dir = new Vector3(0, 0, -1);
            else if (dir == new Vector3(0, 0, -1)) dir = new Vector3(1, 0, 0);
        }
        rigidbody.velocity = dir * speed;
    }
}
