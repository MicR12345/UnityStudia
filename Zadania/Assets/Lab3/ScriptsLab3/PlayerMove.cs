using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    Rigidbody r;
    Vector2 moveV;
    // Start is called before the first frame update
    void Start()
    {
        r = this.gameObject.GetComponent<Rigidbody>();
    }
    void OnMove()
    {
        r.AddForce(new Vector3(1f,0,0));
    }
    private void FixedUpdate()
    {
        r.AddForce(moveV);
    }
}
